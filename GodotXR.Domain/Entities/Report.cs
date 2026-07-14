using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GodotXR.Domain.Entities
{
    public class Report : BaseEntity
    {
        public int AnalyzeId { get; set; }

        public int GeneratedBy { get; set; }

        [MaxLength(50)]
        public string ReportFormat { get; set; } = string.Empty;

        public string? FileUrl { get; set; }

        // Navigation Properties
        [ForeignKey("AnalyzeId")]
        public Analyze Analyze { get; set; } = null!;

        [ForeignKey("GeneratedBy")]
        public User GeneratedByUser { get; set; } = null!;
    }
}