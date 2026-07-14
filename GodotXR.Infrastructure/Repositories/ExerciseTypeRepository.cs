using GodotXR.Domain.Entities;
using GodotXR.Domain.IRepositories;
using GodotXR.Infrastructure.Data;
namespace GodotXR.Infrastructure.Repositories
{
    public class ExerciseTypeRepository : GenericRepository<ExerciseType>, IExerciseTypeRepository
    {
        public ExerciseTypeRepository(AppDbContext context) : base(context) { }
    }
}