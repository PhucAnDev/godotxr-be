using GodotXR.Application.DTOs.Request.ChildSpeechAccuracy;
using GodotXR.Application.DTOs.Response.ChildSpeechAccuracy;

namespace GodotXR.Application.Services
{
    public interface IChildSpeechAccuracyService
    {
        Task<IEnumerable<ChildSpeechAccuracyResponse>> GetByChildIdAsync(int childId);
        Task<IEnumerable<ChildSpeechAccuracyResponse>> GetBySessionIdAsync(string sessionId);
        Task<IEnumerable<ChildSpeechAccuracyResponse>> GetByLessonIdAsync(int lessonId);
        Task<ChildSpeechAccuracyResponse> CreateAsync(CreateChildSpeechAccuracyRequest request);
        Task<int> CreateBatchAsync(IEnumerable<CreateChildSpeechAccuracyRequest> requests);
    }
}
