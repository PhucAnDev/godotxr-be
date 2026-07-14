namespace GodotXR.Application.DTOs.Response.Classroom
{
    public class ClassroomResponse
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string TeacherName { get; set; } = string.Empty;
        public string TeacherSpecialty { get; set; } = string.Empty;
        public int ProgramId { get; set; }
        public string ProgramName { get; set; } = string.Empty;
        public string ProgramLanguage { get; set; } = string.Empty;
        public int TargetAgeFrom { get; set; }
        public int TargetAgeTo { get; set; }
        public int SemesterId { get; set; }
        public string SemesterName { get; set; } = string.Empty;
        public string ClassName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public int EnrollmentCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}