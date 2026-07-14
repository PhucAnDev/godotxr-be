using System.ComponentModel.DataAnnotations;

namespace GodotXR.Application.DTOs.Request.SchoolYear
{
    public class CreateSchoolYearRequest
    {
        [Required, MaxLength(100)]
        public string YearName { get; set; } = string.Empty;
        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required, MaxLength(50)]
        public string Status { get; set; } = "Upcoming";
    }
}