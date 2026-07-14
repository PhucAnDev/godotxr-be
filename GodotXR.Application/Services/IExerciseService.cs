using GodotXR.Application.DTOs.Request.Exercise;
using GodotXR.Application.DTOs.Response;
using GodotXR.Application.DTOs.Response.Exercise;
namespace GodotXR.Application.Services
{
    public interface IExerciseService
    {
        Task<PagedResponse<ExerciseResponse>> GetListAsync(
            int pageNumber, int pageSize,
            int? lessonId = null, int? typeId = null,
            int? teacherId = null, string? status = null);
        Task<ExerciseResponse?> GetByIdAsync(int id);
        Task<(bool Succeeded, IEnumerable<string> Errors, ExerciseResponse? Data)>
            CreateAsync(CreateExerciseRequest request);
        Task<(bool Succeeded, bool NotFound, IEnumerable<string> Errors, ExerciseResponse? Data)>
            UpdateAsync(int id, UpdateExerciseRequest request);
        Task<(bool Succeeded, bool NotFound, IEnumerable<string> Errors)>
            DeleteAsync(int id);
    }
}