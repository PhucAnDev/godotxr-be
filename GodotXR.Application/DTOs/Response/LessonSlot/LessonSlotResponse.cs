using GodotXR.Application.DTOs.Response.ItemAsset;

namespace GodotXR.Application.DTOs.Response.LessonSlot
{
    public class LessonSlotResponse
    {
        public int Id { get; set; }
        public int LessonId { get; set; }
        public int? LessonImageId { get; set; }
        public string SlotName { get; set; } = string.Empty;
        public int? ItemAssetId { get; set; }
        public float CorrectPoints { get; set; }
        public float WrongPoints { get; set; }
        public ItemAssetResponse? ItemAsset { get; set; }
    }
}
