using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GodotXR.Domain.Entities
{
    public class ChildProfile : BaseEntity
    {
        [MaxLength(500)]
        public string? Avatar { get; set; }
        public int UserId { get; set; }

        [Required, MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        public int Age { get; set; }

        [MaxLength(20)]
        public string Gender { get; set; } = string.Empty;

        [MaxLength(50)]
        public string LearningLevel { get; set; } = string.Empty;

        public string? Note { get; set; }

        [MaxLength(50)]
        public string Status { get; set; } = "Active";

        // Navigation Properties
        [ForeignKey("UserId")]
        public User User { get; set; } = null!;

        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
        public ICollection<Result> Results { get; set; } = new List<Result>();
        public ICollection<EventLog> EventLogs { get; set; } = new List<EventLog>();
        public ICollection<Analyze> Analyzes { get; set; } = new List<Analyze>();
    }
}