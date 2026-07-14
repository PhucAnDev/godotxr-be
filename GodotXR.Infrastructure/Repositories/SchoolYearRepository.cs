using GodotXR.Domain.Entities;
using GodotXR.Domain.IRepositories;
using GodotXR.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GodotXR.Infrastructure.Repositories
{
    public class SchoolYearRepository : GenericRepository<SchoolYear>, ISchoolYearRepository
    {
        public SchoolYearRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<bool> HasActiveSchoolYearAsync(int? excludeId = null)
        {
            var query = _context.SchoolYears
                .Where(sy => !sy.IsDeleted && sy.Status == "Active");

            if (excludeId.HasValue)
                query = query.Where(sy => sy.Id != excludeId.Value);

            return await query.AnyAsync();
        }

        public async Task<bool> HasOverlappingAsync(DateTime startDate, DateTime endDate, int? excludeId = null)
        {
            var query = _context.SchoolYears
                .Where(sy =>
                    !sy.IsDeleted &&
                    sy.StartDate < endDate &&
                    sy.EndDate > startDate);

            if (excludeId.HasValue)
                query = query.Where(sy => sy.Id != excludeId.Value);

            return await query.AnyAsync();
        }
    }
}