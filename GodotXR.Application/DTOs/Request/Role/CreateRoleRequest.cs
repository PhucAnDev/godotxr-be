using GodotXR.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace GodotXR.Application.DTOs.Request.Role
{
    public class CreateRoleRequest
    {
        [Required]
        public UserRole RoleName { get; set; }

        [MaxLength(250)]
        public string Description { get; set; } = string.Empty;
    }
}