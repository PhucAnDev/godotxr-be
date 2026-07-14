using GodotXR.Application.DTOs.Response.PronunciationDetail;
using GodotXR.Application.DTOs.Response.EventLog;

namespace GodotXR.Application.DTOs.Response.Result
{
    public sealed class ResultResponse
    {
        public int Id { get; set; }

        public string SessionId { get; set; } = null!;

        public int ChildId { get; set; }

        public int? ExerciseId { get; set; }

        public int? LessonId { get; set; }

        public int AttemptNumber { get; set; }

        public string CompletionStatus { get; set; } = null!;

        public float Score { get; set; }

        public DateTime? StartedAt { get; set; }

        public DateTime? CompletedAt { get; set; }

        public int DurationSeconds { get; set; }

        public string? AudioRecordUrl { get; set; }

        public string? ReplayDataUrl { get; set; }

        public string? InteractionLog { get; set; }

        public string? FeedbackText { get; set; }

        public bool IsFinalized { get; set; }

        public string AudioStatus => string.IsNullOrEmpty(AudioRecordUrl) ? "unavailable" : "available";

        public string ReplayStatus => string.IsNullOrEmpty(ReplayDataUrl) ? "unavailable" : "available";

        public bool HasReplayData => !string.IsNullOrEmpty(ReplayDataUrl);

        public bool HasAudioData => !string.IsNullOrEmpty(AudioRecordUrl);

        public List<PronunciationDetailResponse> PronunciationDetails { get; set; } = new();

        public List<EventLogResponse> EventLogs { get; set; } = new();
    }
}