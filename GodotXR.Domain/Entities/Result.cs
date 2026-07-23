using System.ComponentModel.DataAnnotations.Schema;

namespace GodotXR.Domain.Entities
{
    public class Result : BaseEntity
    {
        public int ChildId { get; set; }

        public int? ExerciseId { get; set; } 

        public int? LessonId { get; set; }

        public ResultType ResultType { get; set; }

        public int AttemptNumber { get; set; }

        public string CompletionStatus { get; set; } = string.Empty;

        public float Score { get; set; }

        public int ErrorCount { get; set; }

        public int CorrectCount { get; set; }

        public DateTime? StartedAt { get; set; }

        public DateTime? CompletedAt { get; set; }

        public int DurationSeconds { get; set; }

        public string? AudioRecordUrl { get; set; }

        public string? ReplayDataUrl { get; set; }

        public string? InteractionLog { get; set; }

        public string? FeedbackText { get; set; }

        public string SessionId { get; set; } = null!;  

        public bool IsFinalized { get; set; } = false;

        // Navigation Properties
        [ForeignKey(nameof(ChildId))]
        public ChildProfile Child { get; set; } = null!;

        [ForeignKey(nameof(ExerciseId))]
        public Exercise? Exercise { get; set; }

        [ForeignKey(nameof(LessonId))]
        public Lesson? Lesson { get; set; }

        public ICollection<PronunciationDetail> PronunciationDetails { get; set; } = new List<PronunciationDetail>();

        public ICollection<EventLog> EventLogs { get; set; } = new List<EventLog>();
    }
}