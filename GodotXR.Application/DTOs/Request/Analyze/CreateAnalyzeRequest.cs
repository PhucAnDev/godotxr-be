using System.ComponentModel.DataAnnotations;

namespace GodotXR.Application.DTOs.Request.Analyze
{
    public sealed class CreateAnalyzeRequest
    {
        [Required]
        public int ChildId { get; set; }

        [Required]
        public SpeechLevel SpeechLevel { get; set; }

        [MaxLength(500)]
        public string? Diagnosis { get; set; }

        [MaxLength(1000)]
        public string? Difficulties { get; set; }

        [MaxLength(1000)]
        public string? Strengths { get; set; }

        [MaxLength(1000)]
        public string? Weaknesses { get; set; }

        [Required]
        public AssessmentLevel CommunicationAbility { get; set; }

        [Required]
        public AssessmentLevel PronunciationAbility { get; set; }

        [Required]
        public AssessmentLevel LanguageComprehension { get; set; }

        [Required]
        public AssessmentLevel LanguageExpression { get; set; }

        [Required]
        public AssessmentLevel AttentionLevel { get; set; }

        [Required]
        public AssessmentLevel SocialInteraction { get; set; }

        [MaxLength(1000)]
        public string? InterventionGoals { get; set; }

        [MaxLength(2000)]
        public string? Recommendation { get; set; }

        [MaxLength(2000)]
        public string? Notes { get; set; }

        [Required]
        public DateTime AssessmentDate { get; set; }

        public DateTime? NextAssessmentDate { get; set; }
    }
}
