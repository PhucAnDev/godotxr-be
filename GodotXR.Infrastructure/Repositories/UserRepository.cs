using GodotXR.Domain.Entities;
using GodotXR.Domain.IRepositories;
using GodotXR.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;


namespace GodotXR.Infrastructure.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<User?> GetByIdWithChildProfilesAsync(int id)
        {
            return await _context.Users
                .Include(u => u.ChildProfiles)
                .FirstOrDefaultAsync(u => u.Id == id);
        }
    }
}
