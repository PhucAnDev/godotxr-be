using GodotXR.Domain.Entities;
using GodotXR.Domain.IRepositories;
using GodotXR.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GodotXR.Infrastructure.Repositories
{
    public class ResultRepository : GenericRepository<Result>, IResultRepository
    {
        public ResultRepository(AppDbContext context) : base(context) { }

        public async Task<Result?> GetBySessionIdAsync(string sessionId)
            => await _context.Results
                .FirstOrDefaultAsync(r => r.SessionId == sessionId && !r.IsDeleted);

        public async Task<int> GetAttemptCountAsync(int childId, int? lessonId)
        {
            var query = _context.Results.Where(r => r.ChildId == childId && !r.IsDeleted);
            return await query.CountAsync(r => r.LessonId == lessonId);
        }

        public async Task<Result?> GetWithDetailsAsync(int id)
            => await _context.Results
                .Include(r => r.PronunciationDetails)
                .Include(r => r.EventLogs)
                .Include(r => r.Child)
                .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);

        public async Task<IEnumerable<Result>> GetByChildIdAsync(int childId)
            => await _context.Results
                .Include(r => r.Lesson)
                .Where(r => r.ChildId == childId && !r.IsDeleted)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
    }
}
