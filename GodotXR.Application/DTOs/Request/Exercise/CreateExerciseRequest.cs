using System.ComponentModel.DataAnnotations;
namespace GodotXR.Application.DTOs.Request.Exercise
{
    public class CreateExerciseRequest
    {
        [Required]
        public int TeacherId { get; set; }
        [Required]
        public int LessonId { get; set; }
        [Required]
        public int TypeId { get; set; }
        [Required, MaxLength(200)]
        public string ExerciseName { get; set; } = string.Empty;
        public string? Instruction { get; set; }
        [Required, MaxLength(50)]
        public string DifficultyLevel { get; set; } = string.Empty;
        [MaxLength(100)]
        public string TargetSkill { get; set; } = string.Empty;
        [Required, MaxLength(50)]
        public string Language { get; set; } = string.Empty;
        [Required]
        public int DurationLimit { get; set; }
        [MaxLength(50)]
        public string Status { get; set; } = "Active";
    }
}