using System.ComponentModel.DataAnnotations;

namespace GodotXR.Domain.Entities
{
    public class Program : BaseEntity
    {
        [Required, MaxLength(200)]
        public string ProgramName { get; set; } = string.Empty;

        public string? Description { get; set; }

        public int TargetAgeFrom { get; set; }

        public int TargetAgeTo { get; set; }

        [MaxLength(50)]
        public string Language { get; set; } = string.Empty;

        [MaxLength(50)]
        public string Status { get; set; } = "Active";
        public ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();

        public ICollection<Classroom> Classrooms { get; set; } = new List<Classroom>();
    }
}