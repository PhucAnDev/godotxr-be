using System.ComponentModel.DataAnnotations;

namespace GodotXR.Application.DTOs.Request.Program
{
    public class CreateProgramRequest
    {
        [Required, MaxLength(200)]
        public string ProgramName { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Range(0, 18)]
        public int TargetAgeFrom { get; set; }

        [Range(0, 18)]
        public int TargetAgeTo { get; set; }

        [Required, MaxLength(50)]
        public string Language { get; set; } = string.Empty;

        [MaxLength(50)]
        public string Status { get; set; } = "Active";
    }
}