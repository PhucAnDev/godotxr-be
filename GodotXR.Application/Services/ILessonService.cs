using GodotXR.Application.DTOs.Request.Lesson;
using GodotXR.Application.DTOs.Response;
using GodotXR.Application.DTOs.Response.Lesson;

namespace GodotXR.Application.Services
{
    public interface ILessonService
    {
        Task<PagedResponse<LessonResponse>> GetListLessonAsync(int pageNumber, int pageSize);

        Task<IEnumerable<LessonResponse>> GetLessonByProgramIdAsync(int programId);

        Task<LessonResponse?> GetLessonByIdAsync(int id);

        Task<(bool Succeeded, IEnumerable<string> Errors, LessonResponse? Data)>
            CreateLessonAsync(CreateLessonRequest request);

        Task<(bool Succeeded, bool NotFound, IEnumerable<string> Errors, LessonResponse? Data)>
            UpdateLessonAsync(int id, UpdateLessonRequest request);

        Task<(bool Succeeded, bool NotFound, IEnumerable<string> Errors)>
            DeleteLessonAsync(int id);
    }
}