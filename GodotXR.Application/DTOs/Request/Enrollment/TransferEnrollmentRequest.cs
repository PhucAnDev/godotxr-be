using System.ComponentModel.DataAnnotations;

namespace GodotXR.Application.DTOs.Request.Enrollment
{
    public class TransferEnrollmentRequest
    {
        [Required]
        public int NewClassId { get; set; }
    }
}