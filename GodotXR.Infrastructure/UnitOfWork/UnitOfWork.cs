using GodotXR.Domain.IRepositories;
using GodotXR.Domain.IUnitOfWork;
using GodotXR.Infrastructure.Data;
using GodotXR.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace GodotXR.Infrastructure.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private IDbContextTransaction? _transaction;
        private readonly Dictionary<Type, object> _repositories = new();
        private IUserRepository? _userRepository;
        private IRoleRepository? _roleRepository;
        private IProgramRepository? _programRepository;
        private ILessonRepository? _lessonRepository;
        private ISchoolYearRepository? _schoolYearRepository;
        private ISemesterRepository? _semesterRepository;
        private IClassroomRepository? _classroomRepository;
        private IChildProfileRepository? _childProfileRepository;
        private IEnrollmentRepository? _enrollmentRepository;
        private IExerciseTypeRepository? _exerciseTypeRepository;
        private IExerciseRepository? _exerciseRepository;
        private IExerciseQuestionRepository? _exerciseQuestionRepository;
        private IResultRepository? _resultRepository;
        private IPronunciationDetailRepository? _pronunciationDetailRepository;
        private IEventLogRepository? _eventLogRepository;
        private IAnalyzeRepository? _analyzeRepository;
        private IReportRepository? _reportRepository;
        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        public IUserRepository UserRepository
        {
            get
            {
                _userRepository ??= new UserRepository(_context);
                return _userRepository;
            }
        }

        public IRoleRepository RoleRepository
        {
            get
            {
                _roleRepository ??= new RoleRepository(_context);
                return _roleRepository;
            }
        }

        public IProgramRepository ProgramRepository
        {
            get
            {
                _programRepository ??= new ProgramRepository(_context);
                return _programRepository;
            }
        }

        public ILessonRepository LessonRepository
        {
            get
            {
                _lessonRepository ??= new LessonRepository(_context);
                return _lessonRepository;
            }
        }

        public ISchoolYearRepository SchoolYearRepository
        {
            get
            {
                _schoolYearRepository ??= new SchoolYearRepository(_context);
                return _schoolYearRepository;
            }
        }
        public ISemesterRepository SemesterRepository
        {
            get
            {
                _semesterRepository ??= new SemesterRepository(_context);
                return _semesterRepository;
            }
        }
        public IClassroomRepository ClassroomRepository
        {
            get
            {
                _classroomRepository ??= new ClassroomRepository(_context);
                return _classroomRepository;
            }
        }

        public IChildProfileRepository ChildProfileRepository
        {
            get
            {
                _childProfileRepository ??= new ChildProfileRepository(_context);
                return _childProfileRepository;
            }
        }

        public IExerciseTypeRepository ExerciseTypeRepository
        {
            get 
            { 
                _exerciseTypeRepository ??= new ExerciseTypeRepository(_context); 
                return _exerciseTypeRepository; 
            }
        }

        public IExerciseRepository ExerciseRepository
        {
            get 
            { 
                _exerciseRepository ??= new ExerciseRepository(_context); 
                return _exerciseRepository; 
            }
        }

        public IExerciseQuestionRepository ExerciseQuestionRepository
        {
            get 
            { 
                _exerciseQuestionRepository ??= new ExerciseQuestionRepository(_context); 
                return _exerciseQuestionRepository; 
            }
        }

        public IResultRepository ResultRepository
        {
            get 
            { 
                _resultRepository ??= new ResultRepository(_context); 
                return _resultRepository; 
            }
        }

        public IPronunciationDetailRepository PronunciationDetailRepository
        {
            get 
            { 
                _pronunciationDetailRepository ??= new PronunciationDetailRepository(_context); 
                return _pronunciationDetailRepository; 
            }
        }

        public IEventLogRepository EventLogRepository
        {
            get 
            { 
                _eventLogRepository ??= new EventLogRepository(_context); 
                return _eventLogRepository; 
            }
        }

        public IAnalyzeRepository AnalyzeRepository
        {
            get
            {
                _analyzeRepository ??= new AnalyzeRepository(_context);
                return _analyzeRepository;
            }
        }

        public IGenericRepository<T> Repository<T>() where T : class
        {
            var type = typeof(T);
            if (!_repositories.TryGetValue(type, out var repository))
            {
                repository = new GenericRepository<T>(_context);
                _repositories[type] = repository;
            }
            return (IGenericRepository<T>)repository;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
                if (_transaction != null)
                    await _transaction.CommitAsync();
            }
            catch
            {
                await RollbackTransactionAsync();
                throw;
            }
            finally
            {
                if (_transaction != null)
                {
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            GC.SuppressFinalize(this);
        }
        public IEnrollmentRepository EnrollmentRepository
        {
            get
            {
                _enrollmentRepository ??= new EnrollmentRepository(_context);
                return _enrollmentRepository;
            }
        }
        public IReportRepository ReportRepository
        {
            get
            {
                _reportRepository ??= new ReportRepository(_context);
                return _reportRepository;
            }
        }
    }
}