using GodotXR.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace GodotXR.Application.DTOs.Request.User
{
    public sealed class CreateAccountRequest
    {
        [Required(ErrorMessage = "FullName là bắt buộc.")]
        [MaxLength(100, ErrorMessage = "FullName không được vượt quá 100 ký tự.")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email là bắt buộc.")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
        [MaxLength(100, ErrorMessage = "Email không được vượt quá 100 ký tự.")]
        public string Email { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Số điện thoại không hợp lệ.")]
        [MaxLength(20, ErrorMessage = "Số điện thoại không được vượt quá 20 ký tự.")]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Gender là bắt buộc.")]
        [MaxLength(20, ErrorMessage = "Gender không được vượt quá 20 ký tự.")]
        public string Gender { get; set; } = string.Empty;

        [Required(ErrorMessage = "Specialty là bắt buộc.")]
        [MaxLength(100, ErrorMessage = "Specialty không được vượt quá 100 ký tự.")]
        public string Specialty { get; set; } = string.Empty;

        [Required(ErrorMessage = "RoleName là bắt buộc.")]
        public UserRole RoleName { get; set; }
    }
}