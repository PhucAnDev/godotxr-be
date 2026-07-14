using System.ComponentModel.DataAnnotations;

namespace GodotXR.Application.DTOs.Request.ExerciseQuestion
{
    public class UpdateExerciseQuestionRequest
    {
        [Required]
        public int ExerciseId { get; set; }
        [Required]
        public int TeacherId { get; set; }
        public string? Instruction { get; set; }
        [Required]
        public string QuestionSentence { get; set; } = string.Empty;
        [Required]
        public string AnswerSentence { get; set; } = string.Empty;
        [Required, MaxLength(50)]
        public string InputType { get; set; } = string.Empty;
        [MaxLength(500)]
        public string? AudioURL { get; set; }
        [MaxLength(500)]
        public string? ImageURL { get; set; }
    }
}