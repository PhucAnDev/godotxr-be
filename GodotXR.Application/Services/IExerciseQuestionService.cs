using GodotXR.Application.DTOs.Request.ExerciseQuestion;
using GodotXR.Application.DTOs.Response;
using GodotXR.Application.DTOs.Response.ExerciseQuestion;
namespace GodotXR.Application.Services
{
    public interface IExerciseQuestionService
    {
        Task<PagedResponse<ExerciseQuestionResponse>> GetListAsync(
            int pageNumber, int pageSize, int? exerciseId = null, int? teacherId = null);
        Task<ExerciseQuestionResponse?> GetByIdAsync(int id);
        Task<(bool Succeeded, IEnumerable<string> Errors, ExerciseQuestionResponse? Data)>
            CreateAsync(CreateExerciseQuestionRequest request);
        Task<(bool Succeeded, bool NotFound, IEnumerable<string> Errors, ExerciseQuestionResponse? Data)>
            UpdateAsync(int id, UpdateExerciseQuestionRequest request);
        Task<(bool Succeeded, bool NotFound, IEnumerable<string> Errors)>
            DeleteAsync(int id);
    }
}