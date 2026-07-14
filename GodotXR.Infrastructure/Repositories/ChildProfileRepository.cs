using GodotXR.Domain.Entities;
using GodotXR.Domain.IRepositories;
using GodotXR.Infrastructure.Data;

namespace GodotXR.Infrastructure.Repositories
{
    public class ChildProfileRepository : GenericRepository<ChildProfile>, IChildProfileRepository
    {
        public ChildProfileRepository(AppDbContext context) : base(context)
        {
        }
    }
}
