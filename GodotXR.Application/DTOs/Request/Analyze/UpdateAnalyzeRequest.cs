using System.ComponentModel.DataAnnotations;

namespace GodotXR.Application.DTOs.Request.Analyze
{
    public sealed class UpdateAnalyzeRequest
    {
        public SpeechLevel? SpeechLevel { get; set; }

        [MaxLength(500)]
        public string? Diagnosis { get; set; }

        [MaxLength(1000)]
        public string? Difficulties { get; set; }

        [MaxLength(1000)]
        public string? Strengths { get; set; }

        [MaxLength(1000)]
        public string? Weaknesses { get; set; }

        public AssessmentLevel? CommunicationAbility { get; set; }

        public AssessmentLevel? PronunciationAbility { get; set; }

        public AssessmentLevel? LanguageComprehension { get; set; }

        public AssessmentLevel? LanguageExpression { get; set; }

        public AssessmentLevel? AttentionLevel { get; set; }

        public AssessmentLevel? SocialInteraction { get; set; }

        [MaxLength(1000)]
        public string? InterventionGoals { get; set; }

        [MaxLength(2000)]
        public string? Recommendation { get; set; }

        [MaxLength(2000)]
        public string? Notes { get; set; }

        public DateTime? AssessmentDate { get; set; }

        public DateTime? NextAssessmentDate { get; set; }
    }
}
