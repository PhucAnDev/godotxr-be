namespace GodotXR.Application.DTOs.Response.Semester
{
    public class SemesterResponse
    {
        public int Id { get; set; }
        public string SemesterName { get; set; } = string.Empty;
        public int SchoolYearId { get; set; }
        public string SchoolYearName { get; set; } = string.Empty;
        public string SchoolYearStatus { get; set; } = string.Empty;
        public DateTime SchoolYearStartDate { get; set; }
        public DateTime SchoolYearEndDate { get; set; }
        public int TeacherId { get; set; }
        public string TeacherName { get; set; } = string.Empty;
        public int ClassCount { get; set; }
        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public int ClassroomCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}