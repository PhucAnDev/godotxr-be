using System;

namespace GodotXR.Application.DTOs.Response.ItemAsset
{
    public class ItemAssetResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string AnswerSentence { get; set; } = string.Empty;
        public string ModelUrl { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public string? AudioUrl { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
