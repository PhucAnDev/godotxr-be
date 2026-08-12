using GodotXR.Application.DTOs.Response.ItemAsset;

namespace GodotXR.Application.DTOs.Response.LessonSlot
{
    public class LessonSlotResponse
    {
        public int Id { get; set; }
        public int LessonId { get; set; }
        public int? LessonImageId { get; set; }
        public string SlotIdentifier { get; set; } = string.Empty;
        public string SlotName { get; set; } = string.Empty;
        public int? ItemAssetId { get; set; }
        public ItemAssetResponse? ItemAsset { get; set; }
    }
}
