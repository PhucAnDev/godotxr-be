namespace GodotXR.Application.DTOs.Response.ChildProfile
{
    /// <summary>
    /// Response DTO cho endpoint GET /api/child-profiles/my-students.
    /// Mở rộng ChildProfileResponse với thông tin lớp học mà học sinh đang enrolled.
    /// </summary>
    public sealed class ChildProfileWithClassResponse
    {
        public int Id { get; set; }

        public string FullName { get; set; } = string.Empty;

        public int Age { get; set; }

        public string Gender { get; set; } = string.Empty;

        public string LearningLevel { get; set; } = string.Empty;

        /// <summary>Trạng thái hồ sơ học sinh (Active / Inactive...)</summary>
        public string Status { get; set; } = string.Empty;

        public string? Avatar { get; set; }

        public string? Note { get; set; }

        public DateTime CreatedAt { get; set; }

        // --- Thông tin lớp học ---

        /// <summary>Id lớp mà học sinh đang được enroll Active.</summary>
        public int ClassroomId { get; set; }

        /// <summary>Tên lớp (ví dụ: "Lớp A1").</summary>
        public string ClassroomName { get; set; } = string.Empty;

        /// <summary>Trạng thái enrollment (luôn là "Active" theo business rule).</summary>
        public string EnrollmentStatus { get; set; } = string.Empty;
    }
}
