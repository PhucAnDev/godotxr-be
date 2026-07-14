using System.ComponentModel.DataAnnotations;

namespace GodotXR.Application.DTOs.Request.Semester
{
    public class CreateSemesterRequest
    {
        [Required, MaxLength(200)]
        public string SemesterName { get; set; } = string.Empty;

        [Required]
        public int SchoolYearId { get; set; }

        [Required]
        public int TeacherId { get; set; }

        [Required]
        public int ClassCount { get; set; }

        public string? Description { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required, MaxLength(50)]
        public string Status { get; set; } = "Upcoming";
    }
}