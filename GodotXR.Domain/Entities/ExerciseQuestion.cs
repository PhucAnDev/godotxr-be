using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GodotXR.Domain.Entities
{
    public class ExerciseQuestion : BaseEntity
    {
        public int ExerciseId { get; set; }

        public int TeacherId { get; set; }

        public string? Instruction { get; set; }

        [Required]
        public string QuestionSentence { get; set; } = string.Empty;

        [Required]
        public string AnswerSentence { get; set; } = string.Empty;

        [MaxLength(50)]
        public string InputType { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? AudioURL { get; set; }

        [MaxLength(500)]
        public string? ImageURL { get; set; }

        // Navigation Properties
        [ForeignKey("ExerciseId")]
        public Exercise Exercise { get; set; } = null!;

        [ForeignKey("TeacherId")]
        public User Teacher { get; set; } = null!;
    }
}