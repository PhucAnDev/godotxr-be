using System.ComponentModel.DataAnnotations.Schema;

namespace GodotXR.Domain.Entities
{
    public class Enrollment : BaseEntity
    {
        public int ChildId { get; set; }

        public int ClassId { get; set; }

        public DateTime EnrollmentDate { get; set; }

        public string Status { get; set; } = "Active";

        // Navigation Properties
        [ForeignKey("ChildId")]
        public ChildProfile Child { get; set; } = null!;

        [ForeignKey("ClassId")]
        public Classroom Classroom { get; set; } = null!;
    }
}