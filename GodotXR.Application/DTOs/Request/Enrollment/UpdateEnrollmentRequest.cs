using System.ComponentModel.DataAnnotations;

namespace GodotXR.Application.DTOs.Request.Enrollment
{
    public class UpdateEnrollmentRequest
    {
        [Required]
        public int ChildId { get; set; }
        [Required]
        public int ClassId { get; set; }
        [Required]
        public DateTime EnrollmentDate { get; set; }
        [Required, MaxLength(50)]
        public string Status { get; set; } = "Active";
    }
}