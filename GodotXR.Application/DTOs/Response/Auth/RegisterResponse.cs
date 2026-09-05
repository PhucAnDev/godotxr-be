using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace GodotXR.Application.DTOs.Response.Auth
{
    public class RegisterResponse
    {
        public int UserId { get; set; }

        public string Email { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;

        public string RoleName { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;
    }
}
