using System.ComponentModel.DataAnnotations;

namespace GodotXR.Application.DTOs.Request.Auth
{
    public sealed class ForgotPasswordRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}
