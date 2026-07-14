namespace GodotXR.Application.DTOs.Response.Analyze
{
    public sealed class AnalyzeResponse
    {
        public int Id { get; set; }

        public int ChildId { get; set; }

        public string? SpeechLevel { get; set; }

        public string? Diagnosis { get; set; }

        public string? Difficulties { get; set; }

        public string? Strengths { get; set; }

        public string? Weaknesses { get; set; }

        public string? CommunicationAbility { get; set; }

        public string? PronunciationAbility { get; set; }

        public string? LanguageComprehension { get; set; }

        public string? LanguageExpression { get; set; }

        public string? AttentionLevel { get; set; }

        public string? SocialInteraction { get; set; }

        public string? InterventionGoals { get; set; }

        public string? Recommendation { get; set; }

        public string? Notes { get; set; }

        public DateTime AssessmentDate { get; set; }

        public DateTime? NextAssessmentDate { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
