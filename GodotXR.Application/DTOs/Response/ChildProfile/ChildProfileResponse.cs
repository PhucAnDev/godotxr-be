namespace GodotXR.Application.DTOs.Response.ChildProfile
{
    public sealed class ChildProfileResponse
    {
        public string? Avatar { get; set; }
        public int Id { get; set; }

        public int UserId { get; set; }

        public string FullName { get; set; } = string.Empty;

        public int Age { get; set; }

        public string Gender { get; set; } = string.Empty;

        public string LearningLevel { get; set; } = string.Empty;

        public string? Note { get; set; }

        public string Status { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
