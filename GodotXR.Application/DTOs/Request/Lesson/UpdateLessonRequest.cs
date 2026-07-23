using System.ComponentModel.DataAnnotations;

namespace GodotXR.Application.DTOs.Request.Lesson
{
    public class UpdateLessonRequest
    {
        [Required, MaxLength(200)]
        public string LessonName { get; set; } = string.Empty;

        [Required]
        public int LessonOrder { get; set; }

        public string? Description { get; set; }

        [MaxLength(100)]
        public string? TargetSkill { get; set; }

        [Required]
        public int EstimatedDuration { get; set; }

        [MaxLength(50)]
        public string Status { get; set; } = "Active";

        public float MaxScore { get; set; } = 100f;
    }
}