using System.ComponentModel.DataAnnotations;

namespace GodotXR.Application.DTOs.Request.LessonSlot
{
    public class ConfigureSlotRequest
    {
        [Required]
        public string SlotIdentifier { get; set; } = string.Empty;
        [Required]
        public string SlotName { get; set; } = string.Empty;
        public int? LessonImageId { get; set; }
    }
}
