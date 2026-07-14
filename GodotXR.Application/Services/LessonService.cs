using AutoMapper;
using GodotXR.Application.DTOs.Request.Lesson;
using GodotXR.Application.DTOs.Response;
using GodotXR.Application.DTOs.Response.Lesson;
using GodotXR.Domain.Entities;
using GodotXR.Domain.IUnitOfWork;


namespace GodotXR.Application.Services
{
    public class LessonService : ILessonService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public LessonService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedResponse<LessonResponse>> GetListLessonAsync(int pageNumber, int pageSize)
        {
            var paged = await _unitOfWork.LessonRepository.GetPagedAsync(pageNumber, pageSize);

            return new PagedResponse<LessonResponse>
            {
                PageNumber = paged.PageNumber,
                PageSize = paged.PageSize,
                TotalCount = paged.TotalCount,
                TotalPages = paged.TotalPages,
                Items = _mapper.Map<List<LessonResponse>>(paged.Items)
            };
        }

        public async Task<IEnumerable<LessonResponse>> GetLessonByProgramIdAsync(int programId)
        {
            var lessons = await _unitOfWork.LessonRepository
                .FindAsync(
                    filter: l => l.ProgramId == programId && !l.IsDeleted,
                    tracked: false);

            return _mapper.Map<IEnumerable<LessonResponse>>(lessons);
        }

        public async Task<LessonResponse?> GetLessonByIdAsync(int id)
        {
            var lesson = await _unitOfWork.LessonRepository
                .GetFirstOrDefaultAsync(
                    filter: l => l.Id == id && !l.IsDeleted,
                    tracked: false);

            return lesson == null
                ? null
                : _mapper.Map<LessonResponse>(lesson);
        }

        public async Task<(bool Succeeded,
                   IEnumerable<string> Errors,
                   LessonResponse? Data)> CreateLessonAsync(CreateLessonRequest request)
        {
            var errors = new List<string>();

            var program = await _unitOfWork.Repository<Program>()
                .GetFirstOrDefaultAsync(
                    filter: p => p.Id == request.ProgramId &&
                                 !p.IsDeleted,
                    tracked: false);

            if (program == null)
                errors.Add("Program not found.");

            if (errors.Any())
                return (false, errors, null);

            var lesson = new Lesson
            {
                ProgramId = request.ProgramId,
                LessonName = request.LessonName,
                LessonOrder = request.LessonOrder,
                Description = request.Description,
                TargetSkill = request.TargetSkill,
                EstimatedDuration = request.EstimatedDuration,
                Status = request.Status,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.LessonRepository.AddAsync(lesson);
            await _unitOfWork.SaveChangesAsync();

            return (
                true,
                Enumerable.Empty<string>(),
                _mapper.Map<LessonResponse>(lesson)
            );
        }

        public async Task<(bool Succeeded,
                   bool NotFound,
                   IEnumerable<string> Errors,
                   LessonResponse? Data)> UpdateLessonAsync(int id, UpdateLessonRequest request)
        {
            var lesson = await _unitOfWork.LessonRepository
                .GetFirstOrDefaultAsync(
                    filter: l => l.Id == id && !l.IsDeleted);

            if (lesson == null)
            {
                return (
                    false,
                    true,
                    Enumerable.Empty<string>(),
                    null
                );
            }

            lesson.LessonName = request.LessonName;
            lesson.LessonOrder = request.LessonOrder;
            lesson.Description = request.Description;
            lesson.TargetSkill = request.TargetSkill;
            lesson.EstimatedDuration = request.EstimatedDuration;
            lesson.Status = request.Status;
            lesson.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync();

            return (
                true,
                false,
                Enumerable.Empty<string>(),
                _mapper.Map<LessonResponse>(lesson)
            );
        }

        public async Task<(bool Succeeded,
                   bool NotFound,
                   IEnumerable<string> Errors)> DeleteLessonAsync(int id)
        {
            var lesson = await _unitOfWork.LessonRepository
                .GetFirstOrDefaultAsync(
                    filter: l => l.Id == id && !l.IsDeleted);

            if (lesson == null)
            {
                return (
                    false,
                    true,
                    Enumerable.Empty<string>()
                );
            }

            lesson.IsDeleted = true;
            lesson.DeletedAt = DateTime.UtcNow;
            lesson.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync();

            return (
                true,
                false,
                Enumerable.Empty<string>()
            );
        }
    }
}
