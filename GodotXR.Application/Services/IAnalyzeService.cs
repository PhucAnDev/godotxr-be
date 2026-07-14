using GodotXR.Application.DTOs.Request.Analyze;
using GodotXR.Application.DTOs.Response;
using GodotXR.Application.DTOs.Response.Analyze;

namespace GodotXR.Application.Services
{
    public interface IAnalyzeService
    {
        Task<PagedResponse<AnalyzeResponse>> GetListAnalyzeAsync(int pageNumber, int pageSize);

        Task<AnalyzeResponse?> GetAnalyzeByIdAsync(int id);

        Task<IEnumerable<AnalyzeResponse>> GetAnalyzesByChildIdAsync(int childId);

        Task<(bool Succeeded, IEnumerable<string> Errors, AnalyzeResponse? Data)>
           CreateAnalyzeAsync(CreateAnalyzeRequest request);

        Task<(bool Succeeded, bool NotFound, IEnumerable<string> Errors, AnalyzeResponse? Data)>
            UpdateAnalyzeAsync(int id, UpdateAnalyzeRequest request);

        Task<(bool Succeeded, bool NotFound, IEnumerable<string> Errors)>
            DeleteAnalyzeAsync(int id);
    }
}
