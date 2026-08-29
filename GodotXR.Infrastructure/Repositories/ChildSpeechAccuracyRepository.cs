using GodotXR.Domain.Entities;
using GodotXR.Domain.IRepositories;
using GodotXR.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GodotXR.Infrastructure.Repositories
{
    public class ChildSpeechAccuracyRepository : GenericRepository<ChildSpeechAccuracy>, IChildSpeechAccuracyRepository
    {
        public ChildSpeechAccuracyRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<ChildSpeechAccuracy>> GetByChildIdAsync(int childId)
            => await _context.ChildSpeechAccuracies
                .Where(csa => csa.ChildProfileId == childId && !csa.IsDeleted)
                .OrderByDescending(csa => csa.CreatedAt)
                .ToListAsync();

        public async Task<IEnumerable<ChildSpeechAccuracy>> GetBySessionIdAsync(string sessionId)
            => await _context.ChildSpeechAccuracies
                .Where(csa => csa.SessionId == sessionId && !csa.IsDeleted)
                .OrderByDescending(csa => csa.CreatedAt)
                .ToListAsync();

        public async Task<IEnumerable<ChildSpeechAccuracy>> GetByLessonIdAsync(int lessonId)
            => await _context.ChildSpeechAccuracies
                .Where(csa => csa.LessonId == lessonId && !csa.IsDeleted)
                .OrderByDescending(csa => csa.CreatedAt)
                .ToListAsync();
    }
}
