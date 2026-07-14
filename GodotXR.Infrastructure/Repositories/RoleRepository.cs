using GodotXR.Domain.IRepositories;
using GodotXR.Infrastructure.Data;
using GodotXR.Domain.Entities;

namespace GodotXR.Infrastructure.Repositories
{
    public class RoleRepository : GenericRepository<Role>, IRoleRepository
    {
        public RoleRepository(AppDbContext context) : base(context)
        {
        }
    }
}
