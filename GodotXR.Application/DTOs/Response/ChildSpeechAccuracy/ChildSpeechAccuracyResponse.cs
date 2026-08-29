namespace GodotXR.Application.DTOs.Response.ChildSpeechAccuracy
{
    public class ChildSpeechAccuracyResponse
    {
        public int Id { get; set; }
        public int ChildProfileId { get; set; }
        public int? LessonId { get; set; }
        public int? LessonSlotId { get; set; }
        public int? ResultId { get; set; }
        public string? SessionId { get; set; }
        public string Word { get; set; } = string.Empty;
        public float AccuracyScore { get; set; }
        public float? FluencyScore { get; set; }
        public float? PronunciationScore { get; set; }
        public float? CompletenessScore { get; set; }
        public string? ErrorType { get; set; }
        public int? AudioChunkIndex { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
