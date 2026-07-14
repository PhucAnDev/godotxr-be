using System.ComponentModel.DataAnnotations;

namespace GodotXR.Application.DTOs.Request.ChildProfile
{
    public sealed class CreateChildProfileRequest
    {
        [MaxLength(500)]
        public string? Avatar { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required, MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        public int Age { get; set; }

        [MaxLength(20)]
        public string Gender { get; set; } = string.Empty;

        [MaxLength(50)]
        public string LearningLevel { get; set; } = string.Empty;

        public string? Note { get; set; }

        [MaxLength(50)]
        public string Status { get; set; } = "Active";
    }
}
