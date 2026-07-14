using GodotXR.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GodotXR.Infrastructure.Configurations
{
    public static class FluentApiConfiguration
    {
        public static void Configure(ModelBuilder modelBuilder)
        {
            // =========================================================
            // IDENTITY
            // =========================================================
            modelBuilder.Entity<Role>()
                .Property(r => r.RoleName)
                .HasConversion<string>()
                .HasMaxLength(50);

            modelBuilder.Entity<User>()
               .HasOne(u => u.Role)
               .WithMany(r => r.Users)
               .HasForeignKey(u => u.RoleId)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Program>()
             .HasMany(p => p.Lessons)
             .WithOne(l => l.Program)
             .HasForeignKey(l => l.ProgramId)
             .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Program>()
                .HasQueryFilter(p => !p.IsDeleted);

            modelBuilder.Entity<Lesson>()
                .HasQueryFilter(l => !l.IsDeleted);

            // =========================================================
            // QUẢN LÝ LỚP HỌC (SchoolYear - Semester - Classroom)
            // =========================================================
            modelBuilder.Entity<SchoolYear>()
                .HasMany(sy => sy.Semesters)
                .WithOne(s => s.SchoolYear)
                .HasForeignKey(s => s.SchoolYearId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Semester>()
                .HasOne(s => s.Teacher)
                .WithMany(u => u.Semesters)
                .HasForeignKey(s => s.TeacherId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Semester>()
                .HasMany(s => s.Classrooms)
                .WithOne(c => c.Semester)
                .HasForeignKey(c => c.SemesterId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Classroom>()
                .HasOne(c => c.User)
                .WithMany(u => u.Classrooms)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Classroom>()
                .HasOne(c => c.Program)
                .WithMany(p => p.Classrooms)
                .HasForeignKey(c => c.ProgramId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SchoolYear>().HasQueryFilter(sy => !sy.IsDeleted);
            modelBuilder.Entity<Semester>().HasQueryFilter(s => !s.IsDeleted);
            modelBuilder.Entity<Classroom>().HasQueryFilter(c => !c.IsDeleted);

            // =========================================================
            // CHILD PROFILE & ENROLLMENT
            // =========================================================
            modelBuilder.Entity<ChildProfile>()
                .HasOne(cp => cp.User)
                .WithMany(u => u.ChildProfiles)
                .HasForeignKey(cp => cp.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Child)
                .WithMany(cp => cp.Enrollments)
                .HasForeignKey(e => e.ChildId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Classroom)
                .WithMany(c => c.Enrollments)
                .HasForeignKey(e => e.ClassId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ChildProfile>().HasQueryFilter(cp => !cp.IsDeleted);
            modelBuilder.Entity<Enrollment>().HasQueryFilter(e => !e.IsDeleted);

            // =========================================================
            // BÀI TẬP (ExerciseType - Exercise - ExerciseQuestion)
            // =========================================================
            modelBuilder.Entity<Exercise>()
                .HasOne(e => e.Teacher)
                .WithMany(u => u.Exercises)
                .HasForeignKey(e => e.TeacherId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Exercise>()
                .HasOne(e => e.Lesson)
                .WithMany(l => l.Exercises)
                .HasForeignKey(e => e.LessonId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Exercise>()
                .HasOne(e => e.ExerciseType)
                .WithMany(et => et.Exercises)
                .HasForeignKey(e => e.TypeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ExerciseQuestion>()
                .HasOne(eq => eq.Exercise)
                .WithMany(e => e.ExerciseQuestions)
                .HasForeignKey(eq => eq.ExerciseId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ExerciseQuestion>()
                .HasOne(eq => eq.Teacher)
                .WithMany(u => u.ExerciseQuestions)
                .HasForeignKey(eq => eq.TeacherId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ExerciseType>().HasQueryFilter(et => !et.IsDeleted);
            modelBuilder.Entity<Exercise>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<ExerciseQuestion>().HasQueryFilter(eq => !eq.IsDeleted);

            // =========================================================
            // KẾT QUẢ LUYỆN TẬP (Result - PronunciationDetail - EventLog)
            // =========================================================
            modelBuilder.Entity<Result>()
                .HasOne(r => r.Child)
                .WithMany(cp => cp.Results)
                .HasForeignKey(r => r.ChildId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Result>()
                .HasOne(r => r.Exercise)
                .WithMany(e => e.Results)
                .HasForeignKey(r => r.ExerciseId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Result>()
                .HasOne(r => r.Lesson)
                .WithMany(l => l.Results)
                .HasForeignKey(r => r.LessonId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PronunciationDetail>()
                .HasOne(pd => pd.Result)
                .WithMany(r => r.PronunciationDetails)
                .HasForeignKey(pd => pd.ResultId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EventLog>()
                .HasOne(ev => ev.Result)
                .WithMany(r => r.EventLogs)
                .HasForeignKey(ev => ev.ResultId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EventLog>()
                .HasOne(ev => ev.Child)
                .WithMany(cp => cp.EventLogs)
                .HasForeignKey(ev => ev.ChildId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Result>().HasQueryFilter(r => !r.IsDeleted);
            modelBuilder.Entity<PronunciationDetail>().HasQueryFilter(pd => !pd.IsDeleted);
            modelBuilder.Entity<EventLog>().HasQueryFilter(ev => !ev.IsDeleted);

            // =========================================================
            // PHÂN TÍCH & BÁO CÁO (Analyze - Report)
            // =========================================================
            modelBuilder.Entity<Analyze>()
                .HasOne(a => a.Child)
                .WithMany(cp => cp.Analyzes)
                .HasForeignKey(a => a.ChildId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Report>()
                .HasOne(r => r.Analyze)
                .WithMany(a => a.Reports)
                .HasForeignKey(r => r.AnalyzeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Report>()
                .HasOne(r => r.GeneratedByUser)
                .WithMany(u => u.Reports)
                .HasForeignKey(r => r.GeneratedBy)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Analyze>().HasQueryFilter(a => !a.IsDeleted);
            modelBuilder.Entity<Report>().HasQueryFilter(r => !r.IsDeleted);
        }
    }
}