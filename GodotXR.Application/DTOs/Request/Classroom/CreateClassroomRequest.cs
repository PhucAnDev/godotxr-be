using System.ComponentModel.DataAnnotations;

namespace GodotXR.Application.DTOs.Request.Classroom
{
    public class CreateClassroomRequest
    {
        [Required]
        public int UserId { get; set; }       

        [Required]
        public int ProgramId { get; set; }

        [Required]
        public int SemesterId { get; set; }

        [Required, MaxLength(200)]
        public string ClassName { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required, MaxLength(50)]
        public string Status { get; set; } = "Upcoming";
    }
}