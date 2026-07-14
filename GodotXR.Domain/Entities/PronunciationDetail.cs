using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GodotXR.Domain.Entities
{
    public class PronunciationDetail : BaseEntity
    {
        public int ResultId { get; set; }

        [MaxLength(50)]
        public string ExpectedPhoneme { get; set; } = string.Empty;

        [MaxLength(50)]
        public string ActualPhoneme { get; set; } = string.Empty;

        public int AccuracyScore { get; set; }

        [MaxLength(100)]
        public string? IssueType { get; set; }

        public string? ReplayDataUrl { get; set; }

        // Navigation Properties
        [ForeignKey("ResultId")]
        public Result Result { get; set; } = null!;
    }
}