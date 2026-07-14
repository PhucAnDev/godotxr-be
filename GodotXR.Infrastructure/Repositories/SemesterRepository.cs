using GodotXR.Domain.Entities;
using GodotXR.Domain.IRepositories;
using GodotXR.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GodotXR.Infrastructure.Repositories
{
    public class SemesterRepository : GenericRepository<Semester>, ISemesterRepository
    {
        public SemesterRepository(AppDbContext context) : base(context)
        {
        }

        // BR-47: kiểm tra overlap trong cùng SchoolYear
        public async Task<bool> HasOverlappingAsync(
            int schoolYearId,
            DateTime startDate,
            DateTime endDate,
            int? excludeId = null)
        {
            var query = _context.Semesters
                .Where(s =>
                    !s.IsDeleted &&
                    s.SchoolYearId == schoolYearId &&
                    s.StartDate < endDate &&
                    s.EndDate > startDate);

            if (excludeId.HasValue)
                query = query.Where(s => s.Id != excludeId.Value);

            return await query.AnyAsync();
        }
    }
}