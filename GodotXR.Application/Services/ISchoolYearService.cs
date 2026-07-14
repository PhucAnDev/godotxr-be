using GodotXR.Application.DTOs.Request.SchoolYear;
using GodotXR.Application.DTOs.Response;
using GodotXR.Application.DTOs.Response.SchoolYear;

namespace GodotXR.Application.Services
{
    public interface ISchoolYearService
    {
        Task<PagedResponse<SchoolYearResponse>> GetListSchoolYearAsync(
            int pageNumber,
            int pageSize,
            string? status = null,
            string? search = null);

        Task<SchoolYearResponse?> GetSchoolYearByIdAsync(int id);

        Task<(bool Succeeded, IEnumerable<string> Errors, SchoolYearResponse? Data)>
            CreateSchoolYearAsync(CreateSchoolYearRequest request);

        Task<(bool Succeeded, bool NotFound, IEnumerable<string> Errors, SchoolYearResponse? Data)>
            UpdateSchoolYearAsync(int id, UpdateSchoolYearRequest request);

        Task<(bool Succeeded, bool NotFound, IEnumerable<string> Errors)>
            DeleteSchoolYearAsync(int id);

        Task<(bool Succeeded, bool NotFound, IEnumerable<string> Errors, SchoolYearResponse? Data)>
            SetActiveSchoolYearAsync(int id);
    }
}