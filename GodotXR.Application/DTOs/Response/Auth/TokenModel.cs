namespace GodotXR.Application.DTOs.Response.Auth
{
    /// <summary>
    /// Token response cho authentication
    /// </summary>
    public class TokenModel
    {
        public string AccessToken { get; set; } = string.Empty;

        public string RefreshToken { get; set; } = string.Empty;

        public UserAuthInfo? User { get; set; }
    }

    /// <summary>
    /// User information kèm theo token
    /// </summary>
    public class UserAuthInfo
    {
        public int Id { get; set; }

        public string Email { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public string RoleName { get; set; } = string.Empty;

        public bool IsActive { get; set; }

        public bool MustChangePassword { get; set; }
    }
}
