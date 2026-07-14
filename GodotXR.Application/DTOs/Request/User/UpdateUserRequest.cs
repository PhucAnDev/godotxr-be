using GodotXR.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodotXR.Application.DTOs.Request.User
{
    public class UpdateUserRequest
    {
        [MaxLength(500)]
        public string? Avatar { get; set; }
        [MaxLength(100)]
        public string? FullName { get; set; }

        [EmailAddress]
        [MaxLength(100)]
        public string? Email { get; set; }

        [Phone]
        [MaxLength(20)]
        public string? Phone { get; set; }

        public UserRole? RoleName { get; set; }
        [MaxLength(20)]
        public string? Gender { get; set; }

        [MaxLength(100)]
        public string? Specialty { get; set; }

        public bool? IsActive { get; set; }
    }
}
