using GodotXR.Domain.Entities;
using GodotXR.Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;

namespace GodotXR.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Role> Roles { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<Program> Programs { get; set; }

        public DbSet<Lesson> Lessons { get; set; }

        public DbSet<SchoolYear> SchoolYears { get; set; }

        public DbSet<Semester> Semesters { get; set; }

        public DbSet<Classroom> Classrooms { get; set; }

        public DbSet<ChildProfile> ChildProfiles { get; set; }

        public DbSet<Enrollment> Enrollments { get; set; }

        public DbSet<ExerciseType> ExerciseTypes { get; set; }

        public DbSet<Exercise> Exercises { get; set; }

        public DbSet<ExerciseQuestion> ExerciseQuestions { get; set; }

        public DbSet<Result> Results { get; set; }

        public DbSet<PronunciationDetail> PronunciationDetails { get; set; }

        public DbSet<EventLog> EventLogs { get; set; }

        public DbSet<Analyze> Analyzes { get; set; }

        public DbSet<Report> Reports { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply Fluent API Configurations
            FluentApiConfiguration.Configure(modelBuilder);

            // Seeding Data
            DbSeeder.Seed(modelBuilder);
        }
    }
}
