using GodotXR.Application.DTOs.Request.ExerciseType;
using GodotXR.Application.DTOs.Response;
using GodotXR.Application.DTOs.Response.ExerciseType;
namespace GodotXR.Application.Services
{
    public interface IExerciseTypeService
    {
        Task<PagedResponse<ExerciseTypeResponse>> GetListAsync(int pageNumber, int pageSize, bool? isActive = null);
        Task<ExerciseTypeResponse?> GetByIdAsync(int id);
        Task<(bool Succeeded, IEnumerable<string> Errors, ExerciseTypeResponse? Data)>
            CreateAsync(CreateExerciseTypeRequest request);
        Task<(bool Succeeded, bool NotFound, IEnumerable<string> Errors, ExerciseTypeResponse? Data)>
            UpdateAsync(int id, UpdateExerciseTypeRequest request);
        Task<(bool Succeeded, bool NotFound, IEnumerable<string> Errors)>
            DeleteAsync(int id);
    }
}