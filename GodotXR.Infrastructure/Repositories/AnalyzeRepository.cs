using GodotXR.Domain.Entities;
using GodotXR.Domain.IRepositories;
using GodotXR.Infrastructure.Data;

namespace GodotXR.Infrastructure.Repositories
{
    public class AnalyzeRepository : GenericRepository<Analyze>, IAnalyzeRepository
    {
        public AnalyzeRepository(AppDbContext context) : base(context)
        {
        }
    }
}
