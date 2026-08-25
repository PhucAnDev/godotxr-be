using System.Text.Json.Serialization;

namespace GodotXR.Application.DTOs.Response.LessonSlot
{
    public class ClientConfigSlotResponse
    {
        public int Id { get; set; }
        public int LessonId { get; set; }
        public string SlotName { get; set; } = string.Empty;
        public float CorrectPoints { get; set; } = 10f;
        public float WrongPoints { get; set; } = 10f;
        public int? ItemAssetId { get; set; }
        public ClientConfigAssetResponse? ItemAsset { get; set; }
    }

    public class ClientConfigAssetResponse
    {
        public int Id { get; set; }

        [JsonPropertyName("ItemName")]
        public string ItemName { get; set; } = string.Empty;
    }
}
