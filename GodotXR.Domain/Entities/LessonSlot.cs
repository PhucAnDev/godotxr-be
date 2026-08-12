using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GodotXR.Domain.Entities
{
    public class LessonSlot : BaseEntity
    {
        public int LessonId { get; set; }

        public int? LessonImageId { get; set; }

        [Required, MaxLength(150)]
        public string SlotIdentifier { get; set; } = string.Empty;

        [Required, MaxLength(200)]
        public string SlotName { get; set; } = string.Empty;

        public int? ItemAssetId { get; set; }

        [ForeignKey(nameof(LessonId))]
        public Lesson Lesson { get; set; } = null!;

        [ForeignKey(nameof(LessonImageId))]
        public LessonImage? LessonImage { get; set; }

        [ForeignKey(nameof(ItemAssetId))]
        public ItemAsset? ItemAsset { get; set; }
    }
}
