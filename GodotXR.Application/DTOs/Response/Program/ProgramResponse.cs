namespace GodotXR.Application.DTOs.Response.Program
{
    public class ProgramResponse
    {
        public int Id { get; set; }
        public string ProgramName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int TargetAgeFrom { get; set; }
        public int TargetAgeTo { get; set; }
        public string Language { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<LessonSummaryResponse> Lessons { get; set; } = new();
    }
}