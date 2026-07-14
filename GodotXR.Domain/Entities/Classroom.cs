using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GodotXR.Domain.Entities
{
    public class Classroom : BaseEntity
    {
        public int UserId { get; set; }

        public int ProgramId { get; set; }

        public int SemesterId { get; set; }

        [Required, MaxLength(200)]
        public string ClassName { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [MaxLength(50)]
        public string Status { get; set; } = "Active";

        // Navigation Properties
        [ForeignKey("UserId")]
        public User User { get; set; } = null!;

        [ForeignKey("ProgramId")]
        public Program Program { get; set; } = null!;

        [ForeignKey("SemesterId")]
        public Semester Semester { get; set; } = null!;

        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    }
}