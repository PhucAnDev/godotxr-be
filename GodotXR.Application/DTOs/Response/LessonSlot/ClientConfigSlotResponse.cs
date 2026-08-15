using System.Text.Json.Serialization;

namespace GodotXR.Application.DTOs.Response.LessonSlot
{
    public class ClientConfigSlotResponse
    {
        public int Id { get; set; }
        public int LessonId { get; set; }
        public string SlotName { get; set; } = string.Empty;
        public int? ItemAssetId { get; set; }
        public ClientConfigAssetResponse? ItemAsset { get; set; }
    }

    public class ClientConfigAssetResponse
    {
        public int Id { get; set; }

        [JsonPropertyName("ItemName")]
        public string ItemName { get; set; } = string.Empty;

        [JsonPropertyName("AnswerSentence")]
        public string AnswerSentence { get; set; } = string.Empty;

        [JsonPropertyName("ModelUrl")]
        public string ModelUrl { get; set; } = string.Empty;

        [JsonPropertyName("ImageUrl")]
        public string? ImageUrl { get; set; }

        [JsonPropertyName("AudioUrl")]
        public string? AudioUrl { get; set; }
    }
}
