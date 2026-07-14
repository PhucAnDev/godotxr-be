using AutoMapper;
using GodotXR.Application.DTOs.Request.Semester;
using GodotXR.Application.DTOs.Response;
using GodotXR.Application.DTOs.Response.Semester;
using GodotXR.Domain.Entities;
using GodotXR.Domain.IUnitOfWork;

namespace GodotXR.Application.Services
{
    public class SemesterService : ISemesterService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SemesterService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedResponse<SemesterResponse>> GetListSemesterAsync(
            int pageNumber, int pageSize,
            int? schoolYearId = null, string? status = null)
        {
            var paged = await _unitOfWork.SemesterRepository.GetPagedAsync(
                pageNumber, pageSize,
                predicate: s =>
                    !s.IsDeleted &&
                    (!schoolYearId.HasValue || s.SchoolYearId == schoolYearId.Value) &&
                    (string.IsNullOrWhiteSpace(status) || s.Status == status),
                orderBy: q => q.OrderByDescending(s => s.StartDate),
                includeProperties: "SchoolYear,Teacher,Classrooms"
            );

            return new PagedResponse<SemesterResponse>
            {
                PageNumber = paged.PageNumber,
                PageSize = paged.PageSize,
                TotalCount = paged.TotalCount,
                TotalPages = paged.TotalPages,
                Items = _mapper.Map<List<SemesterResponse>>(paged.Items)
            };
        }

        public async Task<SemesterResponse?> GetSemesterByIdAsync(int id)
        {
            var semester = await _unitOfWork.SemesterRepository
                .GetFirstOrDefaultAsync(
                    filter: s => s.Id == id && !s.IsDeleted,
                    includeProperties: "SchoolYear,Teacher,Classrooms",
                    tracked: false);

            return semester == null ? null : _mapper.Map<SemesterResponse>(semester);
        }

        public async Task<(bool Succeeded, IEnumerable<string> Errors, SemesterResponse? Data)>
            CreateSemesterAsync(CreateSemesterRequest request)
        {
            if (request.StartDate >= request.EndDate)
                return (false, new[] { "Start date must be before end date." }, null);

            var schoolYear = await _unitOfWork.SchoolYearRepository
                .GetFirstOrDefaultAsync(
                    filter: sy => sy.Id == request.SchoolYearId && !sy.IsDeleted,
                    tracked: false);

            if (schoolYear == null)
                return (false, new[] { "School year not found." }, null);

            if (request.StartDate < schoolYear.StartDate || request.EndDate > schoolYear.EndDate)
                return (false, new[] { "Semester period must fall within the school year period." }, null);

            var hasOverlap = await _unitOfWork.SemesterRepository
                .HasOverlappingAsync(request.SchoolYearId, request.StartDate, request.EndDate);

            if (hasOverlap)
                return (false, new[] { "Semester period overlaps with an existing semester in the same school year." }, null);

            var teacher = await _unitOfWork.UserRepository
                .GetFirstOrDefaultAsync(
                    filter: u => u.Id == request.TeacherId && !u.IsDeleted && u.IsActive,
                    tracked: false);

            if (teacher == null)
                return (false, new[] { "Teacher not found or inactive." }, null);

            var semester = new Semester
            {
                SemesterName = request.SemesterName.Trim(),
                SchoolYearId = request.SchoolYearId,
                TeacherId = request.TeacherId,
                ClassCount = request.ClassCount,
                Description = request.Description,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                Status = request.Status,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.SemesterRepository.AddAsync(semester);
            await _unitOfWork.SaveChangesAsync();

            var created = await _unitOfWork.SemesterRepository
                .GetFirstOrDefaultAsync(
                    filter: s => s.Id == semester.Id,
                    includeProperties: "SchoolYear,Teacher,Classrooms",
                    tracked: false);

            return (true, Enumerable.Empty<string>(), _mapper.Map<SemesterResponse>(created));
        }

        public async Task<(bool Succeeded, bool NotFound, IEnumerable<string> Errors, SemesterResponse? Data)>
            UpdateSemesterAsync(int id, UpdateSemesterRequest request)
        {
            var semester = await _unitOfWork.SemesterRepository
                .GetFirstOrDefaultAsync(
                    filter: s => s.Id == id && !s.IsDeleted,
                    includeProperties: "Classrooms");

            if (semester == null)
                return (false, true, Enumerable.Empty<string>(), null);

            if (request.StartDate >= request.EndDate)
                return (false, false, new[] { "Start date must be before end date." }, null);

            var schoolYear = await _unitOfWork.SchoolYearRepository
                .GetFirstOrDefaultAsync(
                    filter: sy => sy.Id == request.SchoolYearId && !sy.IsDeleted,
                    tracked: false);

            if (schoolYear == null)
                return (false, false, new[] { "School year not found." }, null);

            if (request.StartDate < schoolYear.StartDate || request.EndDate > schoolYear.EndDate)
                return (false, false, new[] { "Semester period must fall within the school year period." }, null);

            var hasOverlap = await _unitOfWork.SemesterRepository
                .HasOverlappingAsync(request.SchoolYearId, request.StartDate, request.EndDate, excludeId: id);

            if (hasOverlap)
                return (false, false, new[] { "Semester period overlaps with an existing semester in the same school year." }, null);

            var teacher = await _unitOfWork.UserRepository
                .GetFirstOrDefaultAsync(
                    filter: u => u.Id == request.TeacherId && !u.IsDeleted && u.IsActive,
                    tracked: false);

            if (teacher == null)
                return (false, false, new[] { "Teacher not found or inactive." }, null);

            semester.SemesterName = request.SemesterName.Trim();
            semester.SchoolYearId = request.SchoolYearId;
            semester.TeacherId = request.TeacherId;
            semester.ClassCount = request.ClassCount;
            semester.Description = request.Description;
            semester.StartDate = request.StartDate;
            semester.EndDate = request.EndDate;
            semester.Status = request.Status;
            semester.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync();

            var updated = await _unitOfWork.SemesterRepository
                .GetFirstOrDefaultAsync(
                    filter: s => s.Id == id,
                    includeProperties: "SchoolYear,Teacher,Classrooms",
                    tracked: false);

            return (true, false, Enumerable.Empty<string>(), _mapper.Map<SemesterResponse>(updated));
        }

        public async Task<(bool Succeeded, bool NotFound, IEnumerable<string> Errors)>
            DeleteSemesterAsync(int id)
        {
            var semester = await _unitOfWork.SemesterRepository
                .GetFirstOrDefaultAsync(
                    filter: s => s.Id == id && !s.IsDeleted,
                    includeProperties: "Classrooms");

            if (semester == null)
                return (false, true, Enumerable.Empty<string>());

            if (semester.Classrooms.Any(c => !c.IsDeleted))
                return (false, false, new[] { "Cannot delete a semester that has active classrooms." });

            if (semester.Status == "Active")
                return (false, false, new[] { "Cannot delete an active semester." });

            semester.IsDeleted = true;
            semester.DeletedAt = DateTime.UtcNow;
            semester.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync();

            return (true, false, Enumerable.Empty<string>());
        }
    }
}