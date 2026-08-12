using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace GodotXR.Application.DTOs.Request.ItemAsset
{
    public class CreateItemAssetRequest
    {
        [Required, MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required, MaxLength(200)]
        public string AnswerSentence { get; set; } = string.Empty;

        [Required]
        public IFormFile ModelFile { get; set; } = null!;

        public IFormFile? ImageFile { get; set; }

        public IFormFile? AudioFile { get; set; }
    }
}
