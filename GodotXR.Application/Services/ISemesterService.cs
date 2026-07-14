using GodotXR.Application.DTOs.Request.Semester;
using GodotXR.Application.DTOs.Response;
using GodotXR.Application.DTOs.Response.Semester;

namespace GodotXR.Application.Services
{
    public interface ISemesterService
    {
        Task<PagedResponse<SemesterResponse>> GetListSemesterAsync(
            int pageNumber,
            int pageSize,
            int? schoolYearId = null,
            string? status = null);

        Task<SemesterResponse?> GetSemesterByIdAsync(int id);

        Task<(bool Succeeded, IEnumerable<string> Errors, SemesterResponse? Data)>
            CreateSemesterAsync(CreateSemesterRequest request);

        Task<(bool Succeeded, bool NotFound, IEnumerable<string> Errors, SemesterResponse? Data)>
            UpdateSemesterAsync(int id, UpdateSemesterRequest request);

        Task<(bool Succeeded, bool NotFound, IEnumerable<string> Errors)>
            DeleteSemesterAsync(int id);
    }
}