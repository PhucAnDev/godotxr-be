using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GodotXR.Domain.Entities
{
    public class Exercise : BaseEntity
    {
        public int TeacherId { get; set; }

        public int LessonId { get; set; }

        public int TypeId { get; set; }

        [Required, MaxLength(200)]
        public string ExerciseName { get; set; } = string.Empty;

        public string? Instruction { get; set; }

        [MaxLength(50)]
        public string DifficultyLevel { get; set; } = string.Empty;

        [MaxLength(100)]
        public string TargetSkill { get; set; } = string.Empty;

        [MaxLength(50)]
        public string Language { get; set; } = string.Empty;

        public int DurationLimit { get; set; }

        [MaxLength(50)]
        public string Status { get; set; } = "Active";

        // Navigation Properties
        [ForeignKey("TeacherId")]
        public User Teacher { get; set; } = null!;

        [ForeignKey("LessonId")]
        public Lesson Lesson { get; set; } = null!;

        [ForeignKey("TypeId")]
        public ExerciseType ExerciseType { get; set; } = null!;

        public ICollection<ExerciseQuestion> ExerciseQuestions { get; set; } = new List<ExerciseQuestion>();

        public ICollection<Result> Results { get; set; } = new List<Result>();
    }
}