using AutoMapper;
using GodotXR.Application.DTOs.Request.SchoolYear;
using GodotXR.Application.DTOs.Response;
using GodotXR.Application.DTOs.Response.SchoolYear;
using GodotXR.Domain.Entities;
using GodotXR.Domain.IUnitOfWork;

namespace GodotXR.Application.Services
{
    public class SchoolYearService : ISchoolYearService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SchoolYearService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedResponse<SchoolYearResponse>> GetListSchoolYearAsync(
            int pageNumber, int pageSize,
            string? status = null, string? search = null)
        {
            var paged = await _unitOfWork.SchoolYearRepository.GetPagedAsync(
                pageNumber, pageSize,
                predicate: sy =>
                    !sy.IsDeleted &&
                    (string.IsNullOrWhiteSpace(status) || sy.Status == status) &&
                    (string.IsNullOrWhiteSpace(search) || sy.YearName.Contains(search)),
                orderBy: q => q.OrderByDescending(sy => sy.StartDate),
                includeProperties: "Semesters"
            );

            return new PagedResponse<SchoolYearResponse>
            {
                PageNumber = paged.PageNumber,
                PageSize = paged.PageSize,
                TotalCount = paged.TotalCount,
                TotalPages = paged.TotalPages,
                Items = _mapper.Map<List<SchoolYearResponse>>(paged.Items)
            };
        }

        public async Task<SchoolYearResponse?> GetSchoolYearByIdAsync(int id)
        {
            var schoolYear = await _unitOfWork.SchoolYearRepository
                .GetFirstOrDefaultAsync(
                    filter: sy => sy.Id == id && !sy.IsDeleted,
                    includeProperties: "Semesters",
                    tracked: false);

            return schoolYear == null ? null : _mapper.Map<SchoolYearResponse>(schoolYear);
        }

        public async Task<(bool Succeeded, IEnumerable<string> Errors, SchoolYearResponse? Data)>
            CreateSchoolYearAsync(CreateSchoolYearRequest request)
        {
            // BR-43: StartDate không được sau EndDate
            if (request.StartDate >= request.EndDate)
                return (false, new[] { "Ngày bắt đầu phải trước ngày kết thúc học phần." }, null);

            // Validation: Không được trùng thời gian với SchoolYear khác
            var hasOverlap = await _unitOfWork.SchoolYearRepository
                .HasOverlappingAsync(request.StartDate, request.EndDate);

            if (hasOverlap)
                return (false, new[] { "Thời gian niên khóa bị trùng lặp với một niên khóa khác đã tồn tại." }, null);

            // BR-44: Chỉ 1 Active tại một thời điểm
            if (request.Status == "Active")
            {
                var hasActive = await _unitOfWork.SchoolYearRepository.HasActiveSchoolYearAsync();
                if (hasActive)
                    return (false, new[] { "Đã có một niên khóa khác đang ở trạng thái Hoạt động." }, null);
            }

            var schoolYear = new SchoolYear
            {
                YearName = request.YearName.Trim(),
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                Status = request.Status,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.SchoolYearRepository.AddAsync(schoolYear);
            await _unitOfWork.SaveChangesAsync();

            return (true, Enumerable.Empty<string>(), _mapper.Map<SchoolYearResponse>(schoolYear));
        }

        public async Task<(bool Succeeded, bool NotFound, IEnumerable<string> Errors, SchoolYearResponse? Data)>
            UpdateSchoolYearAsync(int id, UpdateSchoolYearRequest request)
        {
            var schoolYear = await _unitOfWork.SchoolYearRepository
                .GetFirstOrDefaultAsync(
                    filter: sy => sy.Id == id && !sy.IsDeleted,
                    includeProperties: "Semesters");

            if (schoolYear == null)
                return (false, true, Enumerable.Empty<string>(), null);

            // BR-43
            if (request.StartDate >= request.EndDate)
                return (false, false, new[] { "Ngày bắt đầu phải trước ngày kết thúc học phần." }, null);

            // Validation: Không được trùng thời gian với SchoolYear khác
            var hasOverlap = await _unitOfWork.SchoolYearRepository
                .HasOverlappingAsync(request.StartDate, request.EndDate, excludeId: id);

            if (hasOverlap)
                return (false, false, new[] { "Thời gian niên khóa bị trùng lặp với một niên khóa khác đã tồn tại." }, null);

            // BR-44
            if (request.Status == "Active" && schoolYear.Status != "Active")
            {
                var hasActive = await _unitOfWork.SchoolYearRepository.HasActiveSchoolYearAsync(excludeId: id);
                if (hasActive)
                    return (false, false, new[] { "Đã có một niên khóa khác đang ở trạng thái Hoạt động." }, null);
            }

            schoolYear.YearName = request.YearName.Trim();
            schoolYear.StartDate = request.StartDate;
            schoolYear.EndDate = request.EndDate;
            schoolYear.Status = request.Status;
            schoolYear.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync();

            return (true, false, Enumerable.Empty<string>(), _mapper.Map<SchoolYearResponse>(schoolYear));
        }

        public async Task<(bool Succeeded, bool NotFound, IEnumerable<string> Errors)>
            DeleteSchoolYearAsync(int id)
        {
            var schoolYear = await _unitOfWork.SchoolYearRepository
                .GetFirstOrDefaultAsync(
                    filter: sy => sy.Id == id && !sy.IsDeleted,
                    includeProperties: "Semesters");

            if (schoolYear == null)
                return (false, true, Enumerable.Empty<string>());

            if (schoolYear.Semesters.Any(s => !s.IsDeleted))
                return (false, false, new[] { "Không thể xóa niên khóa đang có học kỳ hoạt động." });

            if (schoolYear.Status == "Active")
                return (false, false, new[] { "Không thể xóa niên khóa đang hoạt động." });

            schoolYear.IsDeleted = true;
            schoolYear.DeletedAt = DateTime.UtcNow;
            schoolYear.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync();

            return (true, false, Enumerable.Empty<string>());
        }

        public async Task<(bool Succeeded, bool NotFound, IEnumerable<string> Errors, SchoolYearResponse? Data)>
            SetActiveSchoolYearAsync(int id)
        {
            var schoolYear = await _unitOfWork.SchoolYearRepository
                .GetFirstOrDefaultAsync(filter: sy => sy.Id == id && !sy.IsDeleted);

            if (schoolYear == null)
                return (false, true, Enumerable.Empty<string>(), null);

            if (schoolYear.Status == "Active")
                return (false, false, new[] { "Niên khóa này đã ở trạng thái Hoạt động rồi." }, null);

            var currentActives = await _unitOfWork.SchoolYearRepository
                .FindAsync(filter: sy => !sy.IsDeleted && sy.Status == "Active");

            foreach (var active in currentActives)
            {
                active.Status = "Completed";
                active.UpdatedAt = DateTime.UtcNow;
            }

            schoolYear.Status = "Active";
            schoolYear.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync();

            return (true, false, Enumerable.Empty<string>(), _mapper.Map<SchoolYearResponse>(schoolYear));
        }
    }
}