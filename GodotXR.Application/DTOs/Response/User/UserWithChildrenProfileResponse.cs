using GodotXR.Application.DTOs.Response.ChildProfile;

namespace GodotXR.Application.DTOs.Response.User
{
    public sealed class UserWithChildrenProfileResponse
    {
        public string? Avatar { get; set; }
        public int Id { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public string RoleName { get; set; } = string.Empty;

        public bool IsActive { get; set; }

        public string Gender { get; set; } = string.Empty;

        public string Specialty { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public List<ChildProfileResponse> ChildProfiles { get; set; } = [];
    }
}
