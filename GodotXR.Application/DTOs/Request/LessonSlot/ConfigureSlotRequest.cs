using System.ComponentModel.DataAnnotations;

namespace GodotXR.Application.DTOs.Request.LessonSlot
{
    public class ConfigureSlotRequest
    {
        [Required]
        public string SlotName { get; set; } = string.Empty;
        public int? LessonImageId { get; set; }
        public float CorrectPoints { get; set; } = 10f;
        public float WrongPoints { get; set; } = 10f;
    }
}
