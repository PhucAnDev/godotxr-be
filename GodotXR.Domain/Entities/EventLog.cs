using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GodotXR.Domain.Entities
{
    public class EventLog : BaseEntity
    {
        public int ResultId { get; set; }

        public int ChildId { get; set; }

        [Required, MaxLength(100)]
        public string EventType { get; set; } = string.Empty;

        public DateTime EventTime { get; set; }

        public string? Description { get; set; }

        // Navigation Properties
        [ForeignKey("ResultId")]
        public Result Result { get; set; } = null!;

        [ForeignKey("ChildId")]
        public ChildProfile Child { get; set; } = null!;
    }
}