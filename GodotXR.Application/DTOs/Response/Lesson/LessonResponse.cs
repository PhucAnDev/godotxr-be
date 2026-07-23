namespace GodotXR.Application.DTOs.Response.Lesson
{
    public class LessonResponse
    {
        public int Id { get; set; }
        public int ProgramId { get; set; }
        public string LessonName { get; set; } = string.Empty;
        public int LessonOrder { get; set; }
        public string? Description { get; set; }
        public string? TargetSkill { get; set; }
        public int EstimatedDuration { get; set; }
        public string Status { get; set; } = string.Empty;
        public float MaxScore { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}