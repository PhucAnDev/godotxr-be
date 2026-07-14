using GodotXR.Domain.IRepositories;

namespace GodotXR.Domain.IUnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository UserRepository { get; }

        IRoleRepository RoleRepository { get; }

        IProgramRepository ProgramRepository { get; }

        ILessonRepository LessonRepository { get; }

        ISchoolYearRepository SchoolYearRepository { get; }

        ISemesterRepository SemesterRepository { get; }

        IClassroomRepository ClassroomRepository { get; }

        IChildProfileRepository ChildProfileRepository { get; }

        IEnrollmentRepository EnrollmentRepository { get; }

        IExerciseTypeRepository ExerciseTypeRepository { get; }

        IExerciseRepository ExerciseRepository { get; }

        IExerciseQuestionRepository ExerciseQuestionRepository { get; }

        IResultRepository ResultRepository { get; }

        IPronunciationDetailRepository PronunciationDetailRepository { get; }

        IEventLogRepository EventLogRepository { get; }

        IAnalyzeRepository AnalyzeRepository { get; }
        IReportRepository ReportRepository { get; }
        IGenericRepository<T> Repository<T>() where T : class;

        Task<int> SaveChangesAsync();

        Task BeginTransactionAsync();

        Task CommitTransactionAsync();

        Task RollbackTransactionAsync();
    }
}
