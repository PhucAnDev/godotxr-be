namespace GodotXR.Application.DTOs.Response.User
{
    public class CreateAccountResponse
    {
        public int UserId { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string RoleName { get; set; } = string.Empty;

        public string Message { get; set; } = "Tài khoản đã được tạo. Vui lòng kiểm tra email để xác minh tài khoản.";
    }
}