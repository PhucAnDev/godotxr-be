using GodotXR.Domain.Entities;

namespace GodotXR.Domain.IRepositories
{
    public interface IResultRepository : IGenericRepository<Result>
    {
        Task<Result?> GetBySessionIdAsync(string sessionId);

        Task<int> GetAttemptCountAsync(int childId, int? lessonId, int? exerciseId);

        Task<Result?> GetWithDetailsAsync(int id);

        Task<IEnumerable<Result>> GetByChildIdAsync(int childId);

        Task<IEnumerable<Result>> GetByExerciseIdAsync(int exerciseId);
    }
}
