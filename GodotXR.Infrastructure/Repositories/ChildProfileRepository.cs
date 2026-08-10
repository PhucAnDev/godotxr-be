using GodotXR.Domain.Entities;
using GodotXR.Domain.IRepositories;
using GodotXR.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GodotXR.Infrastructure.Repositories
{
    public class ChildProfileRepository : GenericRepository<ChildProfile>, IChildProfileRepository
    {
        public ChildProfileRepository(AppDbContext context) : base(context)
        {
        }

        /// <inheritdoc/>
        public async Task<(IEnumerable<ChildProfileWithClassDto> Items, int TotalCount)>
            GetStudentsByTeacherIdAsync(int teacherId, int pageNumber, int pageSize)
        {
            // Base query: enrollment active → classroom thuộc teacher → child không bị xóa
            var query = _context.Enrollments
                .Include(e => e.Child)
                .Include(e => e.Classroom)
                .Where(e =>
                    !e.IsDeleted &&
                    !e.Child.IsDeleted &&
                    !e.Classroom.IsDeleted &&
                    e.Classroom.UserId == teacherId &&
                    e.Status == "Active")
                .OrderBy(e => e.Child.FullName);

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(e => new ChildProfileWithClassDto(
                    e.Child,
                    e.Classroom.Id,
                    e.Classroom.ClassName,
                    e.Status))
                .ToListAsync();

            return (items, totalCount);
        }
    }
}
