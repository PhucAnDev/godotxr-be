using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace GodotXR.Application.DTOs.Request.ItemAsset
{
    public class UpdateItemAssetRequest
    {
        [Required, MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required, MaxLength(200)]
        public string AnswerSentence { get; set; } = string.Empty;

        public IFormFile? ModelFile { get; set; }

        public IFormFile? ImageFile { get; set; }

        public IFormFile? AudioFile { get; set; }
    }
}
