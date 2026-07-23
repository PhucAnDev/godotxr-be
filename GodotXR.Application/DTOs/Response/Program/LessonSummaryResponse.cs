namespace GodotXR.Application.DTOs.Response.Program
{
    public class LessonSummaryResponse
    {
        public int Id { get; set; }
        public string LessonName { get; set; } = string.Empty;
        public int LessonOrder { get; set; }
        public string? TargetSkill { get; set; }
        public int EstimatedDuration { get; set; }
        public string Status { get; set; } = string.Empty;
        public float MaxScore { get; set; }
    }
}