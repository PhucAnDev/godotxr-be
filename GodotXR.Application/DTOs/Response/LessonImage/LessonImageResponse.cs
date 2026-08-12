namespace GodotXR.Application.DTOs.Response.LessonImage
{
    public class LessonImageResponse
    {
        public int Id { get; set; }
        public int LessonId { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string AngleName { get; set; } = string.Empty;
    }
}
