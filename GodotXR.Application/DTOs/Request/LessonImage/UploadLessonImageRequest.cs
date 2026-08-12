using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace GodotXR.Application.DTOs.Request.LessonImage
{
    public class UploadLessonImageRequest
    {
        [Required]
        public string AngleName { get; set; } = string.Empty;
        [Required]
        public IFormFile ImageFile { get; set; } = null!;
    }
}
