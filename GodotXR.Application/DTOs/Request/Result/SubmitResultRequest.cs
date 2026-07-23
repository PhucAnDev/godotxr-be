namespace GodotXR.Application.DTOs.Request.Result
{
    public sealed class SubmitResultRequest
    {
        public string SessionId { get; set; } = null!;

        public int ChildId { get; set; }

        public int? ExerciseId { get; set; }

        public int? LessonId { get; set; }

        public string CompletionStatus { get; set; } = null!; 

        public float Score { get; set; }

        public int ErrorCount { get; set; }

        public int CorrectCount { get; set; }

        public DateTime StartedAt { get; set; }

        public DateTime? CompletedAt { get; set; }

        public int DurationSeconds { get; set; }

        public string? InteractionLog { get; set; }

        public string? FeedbackText { get; set; }
    }
}
