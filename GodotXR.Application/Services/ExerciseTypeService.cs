using AutoMapper;
using GodotXR.Application.DTOs.Request.ExerciseType;
using GodotXR.Application.DTOs.Response;
using GodotXR.Application.DTOs.Response.ExerciseType;
using GodotXR.Domain.Entities;
using GodotXR.Domain.IUnitOfWork;

namespace GodotXR.Application.Services
{
    public class ExerciseTypeService : IExerciseTypeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ExerciseTypeService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedResponse<ExerciseTypeResponse>> GetListAsync(
            int pageNumber, int pageSize, bool? isActive = null)
        {
            var paged = await _unitOfWork.ExerciseTypeRepository.GetPagedAsync(
                pageNumber, pageSize,
                predicate: t => !t.IsDeleted
                             && (!isActive.HasValue || t.IsActive == isActive.Value),
                orderBy: q => q.OrderBy(t => t.TypeName));

            return new PagedResponse<ExerciseTypeResponse>
            {
                PageNumber = paged.PageNumber,
                PageSize = paged.PageSize,
                TotalCount = paged.TotalCount,
                TotalPages = paged.TotalPages,
                Items = _mapper.Map<List<ExerciseTypeResponse>>(paged.Items)
            };
        }

        public async Task<ExerciseTypeResponse?> GetByIdAsync(int id)
        {
            var entity = await _unitOfWork.ExerciseTypeRepository
                .GetFirstOrDefaultAsync(
                    filter: t => t.Id == id && !t.IsDeleted,
                    tracked: false);
            return entity == null ? null : _mapper.Map<ExerciseTypeResponse>(entity);
        }

        public async Task<(bool Succeeded, IEnumerable<string> Errors, ExerciseTypeResponse? Data)>
            CreateAsync(CreateExerciseTypeRequest request)
        {
            // Duplicate check
            var duplicate = await _unitOfWork.ExerciseTypeRepository
                .GetFirstOrDefaultAsync(
                    filter: t => t.TypeName == request.TypeName.Trim() && !t.IsDeleted,
                    tracked: false);
            if (duplicate != null)
                return (false, new[] { "Exercise type name already exists." }, null);

            var entity = new ExerciseType
            {
                TypeName = request.TypeName.Trim(),
                Description = request.Description,
                IsActive = request.IsActive,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.ExerciseTypeRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return (true, Enumerable.Empty<string>(), _mapper.Map<ExerciseTypeResponse>(entity));
        }

        public async Task<(bool Succeeded, bool NotFound, IEnumerable<string> Errors, ExerciseTypeResponse? Data)>
            UpdateAsync(int id, UpdateExerciseTypeRequest request)
        {
            var entity = await _unitOfWork.ExerciseTypeRepository
                .GetFirstOrDefaultAsync(
                    filter: t => t.Id == id && !t.IsDeleted,
                    includeProperties: "Exercises");
            if (entity == null)
                return (false, true, Enumerable.Empty<string>(), null);

            // BR-62: Không được deactivate khi còn Exercise Active đang dùng
            if (!request.IsActive && entity.IsActive)
            {
                var hasActiveExercises = entity.Exercises
                    .Any(e => !e.IsDeleted && e.Status == "Active");
                if (hasActiveExercises)
                    return (false, false,
                        new[] { "Cannot deactivate an exercise type that has active exercises." },
                        null);
            }

            // Duplicate check (exclude self)
            var duplicate = await _unitOfWork.ExerciseTypeRepository
                .GetFirstOrDefaultAsync(
                    filter: t => t.TypeName == request.TypeName.Trim()
                              && !t.IsDeleted
                              && t.Id != id,
                    tracked: false);
            if (duplicate != null)
                return (false, false, new[] { "Exercise type name already exists." }, null);

            entity.TypeName = request.TypeName.Trim();
            entity.Description = request.Description;
            entity.IsActive = request.IsActive;
            entity.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync();

            return (true, false, Enumerable.Empty<string>(), _mapper.Map<ExerciseTypeResponse>(entity));
        }

        public async Task<(bool Succeeded, bool NotFound, IEnumerable<string> Errors)>
            DeleteAsync(int id)
        {
            var entity = await _unitOfWork.ExerciseTypeRepository
                .GetFirstOrDefaultAsync(
                    filter: t => t.Id == id && !t.IsDeleted,
                    includeProperties: "Exercises");
            if (entity == null)
                return (false, true, Enumerable.Empty<string>());

            // Block nếu còn Exercise chưa xóa
            if (entity.Exercises.Any(e => !e.IsDeleted))
                return (false, false,
                    new[] { "Cannot delete an exercise type that has exercises." });

            entity.IsDeleted = true;
            entity.DeletedAt = DateTime.UtcNow;
            entity.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync();
            return (true, false, Enumerable.Empty<string>());
        }
    }
}