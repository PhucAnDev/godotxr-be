using System.ComponentModel.DataAnnotations;

namespace GodotXR.Domain.Entities
{
    public class Lesson : BaseEntity
    {
        public int ProgramId { get; set; }

        [Required, MaxLength(200)]
        public string LessonName { get; set; } = string.Empty;

        public int LessonOrder { get; set; }

        public string? Description { get; set; }

        [MaxLength(100)]
        public string? TargetSkill { get; set; }

        public int EstimatedDuration { get; set; }

        [MaxLength(50)]
        public string Status { get; set; } = "Active";

        public Program Program { get; set; } = null!;

        public ICollection<Exercise> Exercises { get; set; } = new List<Exercise>();

        public ICollection<Result> Results { get; set; } = new List<Result>();
    }
}