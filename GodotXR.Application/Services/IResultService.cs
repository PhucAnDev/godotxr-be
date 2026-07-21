using GodotXR.Application.DTOs.Request.Result;
using GodotXR.Application.DTOs.Response;
using GodotXR.Application.DTOs.Response.Result;

namespace GodotXR.Application.Services
{
    public interface IResultService
    {
        Task<(bool Succeeded, bool NotFound, IEnumerable<string> Errors, ResultResponse? Data)> SubmitAsync(SubmitResultRequest request);

        Task<ApiResponse<ResultResponse>> GetByIdAsync(int id);

        Task<ApiResponse<IEnumerable<ResultResponse>>> GetByChildIdAsync(int childId);

        Task<ApiResponse<IEnumerable<ResultResponse>>> GetByExerciseIdAsync(int exerciseId);

        Task<ApiResponse<ResultResponse>> UpdateFeedbackAsync(int id, string feedbackText);
    }
}