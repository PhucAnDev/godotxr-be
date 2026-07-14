using GodotXR.Domain.Entities;
using GodotXR.Domain.IRepositories;
using GodotXR.Infrastructure.Data;
namespace GodotXR.Infrastructure.Repositories
{
    public class ExerciseQuestionRepository : GenericRepository<ExerciseQuestion>, IExerciseQuestionRepository
    {
        public ExerciseQuestionRepository(AppDbContext context) : base(context) { }
    }
}