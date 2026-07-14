using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GodotXR.Domain.Entities
{
    public class Semester : BaseEntity
    {
        [Required, MaxLength(200)]
        public string SemesterName { get; set; } = string.Empty;
        public int SchoolYearId { get; set; }

        public int TeacherId { get; set; }

        public int ClassCount { get; set; }

        public string? Description { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [MaxLength(50)]
        public string Status { get; set; } = "Active";

        // Navigation Properties
        [ForeignKey("SchoolYearId")]
        public SchoolYear SchoolYear { get; set; } = null!;

        [ForeignKey("TeacherId")]
        public User Teacher { get; set; } = null!;

        public ICollection<Classroom> Classrooms { get; set; } = new List<Classroom>();
    }
}