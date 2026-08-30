using GodotXR.Domain.Entities;

namespace GodotXR.Domain.IRepositories
{
    public interface IChildSpeechAccuracyRepository : IGenericRepository<ChildSpeechAccuracy>
    {
        Task<IEnumerable<ChildSpeechAccuracy>> GetByChildIdAsync(int childId);
        Task<IEnumerable<ChildSpeechAccuracy>> GetBySessionIdAsync(string sessionId);
        Task<IEnumerable<ChildSpeechAccuracy>> GetByLessonIdAsync(int lessonId);
        Task<IEnumerable<ChildSpeechAccuracy>> GetByChunkAsync(int childProfileId, string sessionId, int audioChunkIndex);
    }
}
