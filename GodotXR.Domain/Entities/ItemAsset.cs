using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace GodotXR.Domain.Entities
{
    public class ItemAsset : BaseEntity
    {
        [Required, MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required, MaxLength(200)]
        public string AnswerSentence { get; set; } = string.Empty;

        [Required, MaxLength(500)]
        public string ModelUrl { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? ImageUrl { get; set; }

        [MaxLength(500)]
        public string? AudioUrl { get; set; }

        public ICollection<LessonSlot> LessonSlots { get; set; } = new List<LessonSlot>();
    }
}
