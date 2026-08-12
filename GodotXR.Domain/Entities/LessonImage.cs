using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace GodotXR.Domain.Entities
{
    public class LessonImage : BaseEntity
    {
        public int LessonId { get; set; }

        [Required, MaxLength(500)]
        public string ImageUrl { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string AngleName { get; set; } = string.Empty;

        [ForeignKey(nameof(LessonId))]
        public Lesson Lesson { get; set; } = null!;

        public ICollection<LessonSlot> LessonSlots { get; set; } = new List<LessonSlot>();
    }
}
