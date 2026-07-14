namespace GodotXR.Application.DTOs.Response.SchoolYear
{
    public class SchoolYearResponse
    {
        public int Id { get; set; }
        public string YearName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public int ClassCount { get; set; }
        public int SemesterCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}