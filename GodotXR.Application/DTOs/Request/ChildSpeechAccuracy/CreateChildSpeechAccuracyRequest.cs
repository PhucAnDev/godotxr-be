using System.ComponentModel.DataAnnotations;

namespace GodotXR.Application.DTOs.Request.ChildSpeechAccuracy
{
    public class CreateChildSpeechAccuracyRequest
    {
        [Required]
        public int ChildProfileId { get; set; }

        public int? LessonId { get; set; }

        public int? LessonSlotId { get; set; }

        public int? ResultId { get; set; }

        [MaxLength(100)]
        public string? SessionId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Word { get; set; } = string.Empty;

        public float AccuracyScore { get; set; }

        public float? FluencyScore { get; set; }

        public float? PronunciationScore { get; set; }

        public float? CompletenessScore { get; set; }

        [MaxLength(50)]
        public string? ErrorType { get; set; }

        public int? AudioChunkIndex { get; set; }
    }
}
