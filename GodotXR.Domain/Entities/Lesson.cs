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

        public float MaxScore { get; set; } = 100f;

        public float CompletionBonusPoints { get; set; } = 20f;

        public float CorrectAnswerScore { get; set; } = 10f;

        public float IncorrectAnswerScore { get; set; } = 0f;

        public string? Note { get; set; }

        public Program Program { get; set; } = null!;

        public ICollection<LessonImage> LessonImages { get; set; } = new List<LessonImage>();

        public ICollection<LessonSlot> LessonSlots { get; set; } = new List<LessonSlot>();

        public ICollection<Result> Results { get; set; } = new List<Result>();
    }
}