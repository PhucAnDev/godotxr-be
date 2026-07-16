using AutoMapper;
using GodotXR.Application.DTOs.Request.User;
using GodotXR.Application.DTOs.Response;
using GodotXR.Application.DTOs.Response.User;
using GodotXR.Domain.Entities;
using GodotXR.Domain.IUnitOfWork;
using Microsoft.Extensions.Configuration;

namespace GodotXR.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMailService _mailService;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public UserService(
            IUnitOfWork unitOfWork,
            IMailService mailService,
            IConfiguration configuration,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mailService = mailService;
            _configuration = configuration;
            _mapper = mapper;
        }

        public async Task<PagedResponse<UserResponse>> GetListUserAsync(int pageNumber, int pageSize)
        {
            var paged = await _unitOfWork.UserRepository.GetPagedAsync(
                pageNumber,
                pageSize,
                r => r.IsActive && !r.IsDeleted,
                includeProperties: "Role");

            return new PagedResponse<UserResponse>
            {
                PageNumber = paged.PageNumber,
                PageSize = paged.PageSize,
                TotalCount = paged.TotalCount,
                TotalPages = paged.TotalPages,
                Items = paged.Items.Select(MapToResponse).ToList(),
            };
        }

        public async Task<UserResponse?> GetUserByIdAsync(int id)
        {
            var user = await _unitOfWork.UserRepository.GetFirstOrDefaultAsync(
                filter: u => u.Id == id && !u.IsDeleted,
                includeProperties: "Role",
                tracked: false);

            return user == null ? null : MapToResponse(user);
        }

        public async Task<(bool Succeeded, IEnumerable<string> Errors, UserResponse? Data)> CreateUserAsync(CreateUserRequest request)
        {
            var errors = new List<string>();

            var emailExists = await _unitOfWork.UserRepository.ExistsAsync(
                u => u.Email == request.Email && !u.IsDeleted);

            if (emailExists)
                errors.Add($"Email '{request.Email}' đã tồn tại.");

            var role = await _unitOfWork.RoleRepository.GetFirstOrDefaultAsync(
                r => r.RoleName == request.RoleName && r.IsActive);

            if (role == null)
                errors.Add($"Role '{request.RoleName}' không tồn tại hoặc không hoạt động.");

            if (errors.Any())
                return (false, errors, null);

            var user = new User
            {
                Avatar = request.Avatar,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                FullName = request.FullName,
                Email = request.Email,
                Phone = request.Phone,
                Gender = request.Gender,
                Specialty = request.Specialty,
                RoleId = role!.Id,
                IsActive = true
            };

            await _unitOfWork.UserRepository.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            var createdUser = await _unitOfWork.UserRepository.GetFirstOrDefaultAsync(
                u => u.Id == user.Id,
                includeProperties: "Role",
                tracked: false);

            return (true, Enumerable.Empty<string>(), MapToResponse(createdUser!));
        }

        public async Task<(bool Succeeded, IEnumerable<string> Errors, CreateAccountResponse? Data)> CreateAccountAsync(CreateAccountRequest request)
        {
            var errors = new List<string>();

            var emailExists = await _unitOfWork.UserRepository.ExistsAsync(
                u => u.Email == request.Email && !u.IsDeleted);

            if (emailExists)
                errors.Add($"Email '{request.Email}' đã tồn tại.");

            var role = await _unitOfWork.RoleRepository.GetFirstOrDefaultAsync(
                r => r.RoleName == request.RoleName && r.IsActive);

            if (role == null)
                errors.Add($"Role '{request.RoleName}' không tồn tại hoặc không hoạt động.");

            if (errors.Any())
                return (false, errors, null);

            var temporaryPassword = GenerateTemporaryPassword();
            var verifyToken = GenerateVerifyToken();

            var user = new User
            {
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(temporaryPassword),
                FullName = request.FullName,
                Email = request.Email,
                Phone = request.Phone,
                Gender = request.Gender,
                Specialty = request.Specialty,
                RoleId = role!.Id,
                IsActive = false,
                IsEmailVerified = false,
                MustChangePassword = true,
                VerifyToken = verifyToken,
                VerifyTokenExpiry = DateTime.UtcNow.AddHours(24)
            };

            await _unitOfWork.UserRepository.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();
            var frontendUrl = _configuration["App:FrontendBaseUrl"] ?? "http://localhost:3000";
            var verifyLink = $"{frontendUrl}/verify-email?token={Uri.EscapeDataString(verifyToken)}";

            try
            {
                var emailBody = GenerateAccountCreationEmailBody(
                    request.FullName,
                    request.Email,
                    temporaryPassword,
                    verifyLink);

                await _mailService.SendEmailAsync(
                    request.Email,
                    "Tài khoản mới - Xác minh Email",
                    emailBody);
            }
            catch (Exception ex)
            {
                return (false, new[] { $"Gửi email thất bại: {ex.Message}" }, null);
            }

            return (true, Enumerable.Empty<string>(), new CreateAccountResponse
            {
                UserId = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                RoleName = role.RoleName.ToString(),
                Message = "Tài khoản đã được tạo. Email xác minh đã được gửi đến người dùng."
            });
        }

        public async Task<(bool Succeeded, bool NotFound, IEnumerable<string> Errors, UserResponse? Data)> UpdateUserAsync(int id, UpdateUserRequest request)
        {
            var user = await _unitOfWork.UserRepository.GetFirstOrDefaultAsync(
                filter: u => u.Id == id && !u.IsDeleted,
                includeProperties: "Role");

            if (user == null)
                return (false, true, Enumerable.Empty<string>(), null);

            var errors = new List<string>();

            if (!string.IsNullOrWhiteSpace(request.Email) && request.Email != user.Email)
            {
                var emailExists = await _unitOfWork.UserRepository.ExistsAsync(
                    u => u.Email == request.Email && u.Id != id && !u.IsDeleted);

                if (emailExists)
                    errors.Add($"Email '{request.Email}' đã được dùng bởi tài khoản khác.");
            }

            if (request.RoleName.HasValue)
            {
                var role = await _unitOfWork.RoleRepository.GetFirstOrDefaultAsync(
                    r => r.RoleName == request.RoleName.Value && r.IsActive);

                if (role == null)
                    errors.Add($"Role '{request.RoleName}' không tồn tại hoặc không hoạt động.");
                else
                {
                    user.RoleId = role.Id;
                    user.Role = role;
                }
            }

            if (errors.Any())
                return (false, false, errors, null);

            if (!string.IsNullOrWhiteSpace(request.Avatar))
                user.Avatar = request.Avatar;

            if (!string.IsNullOrWhiteSpace(request.FullName))
                user.FullName = request.FullName;

            if (!string.IsNullOrWhiteSpace(request.Email))
                user.Email = request.Email;

            if (!string.IsNullOrWhiteSpace(request.Gender))
                user.Gender = request.Gender;

            if (!string.IsNullOrWhiteSpace(request.Specialty))
                user.Specialty = request.Specialty;

            if (!string.IsNullOrWhiteSpace(request.Phone))
                user.Phone = request.Phone;

            if (request.IsActive.HasValue)
                user.IsActive = request.IsActive.Value;

            user.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.UserRepository.Update(user);
            await _unitOfWork.SaveChangesAsync();

            return (true, false, Enumerable.Empty<string>(), MapToResponse(user));
        }

        public async Task<(bool Succeeded, bool NotFound, IEnumerable<string> Errors)> DeleteUserAsync(int id)
        {
            var user = await _unitOfWork.UserRepository.GetFirstOrDefaultAsync(
                u => u.Id == id && !u.IsDeleted);

            if (user == null)
                return (false, true, Enumerable.Empty<string>());

            user.IsDeleted = true;
            user.DeletedAt = DateTime.UtcNow;
            user.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.UserRepository.Update(user);
            await _unitOfWork.SaveChangesAsync();

            return (true, false, Enumerable.Empty<string>());
        }

        public async Task<(bool Succeeded, bool NotFound, IEnumerable<string> Errors, UserWithChildrenProfileResponse? Data)>
            GetCurrentUserWithChildrenProfilesAsync(int userId)
        {
            var user = await _unitOfWork.UserRepository.GetByIdWithChildProfilesAsync(userId);

            if (user is null)
            {
                return (
                    Succeeded: false,
                    NotFound: true,
                    Errors: ["User not found."],
                    Data: null
                );
            }

            var response = _mapper.Map<UserWithChildrenProfileResponse>(user);

            return (
                Succeeded: true,
                NotFound: false,
                Errors: Enumerable.Empty<string>(),
                Data: response
            );
        }

        private static UserResponse MapToResponse(User user) => new()
        {
            Avatar = user.Avatar,
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Phone = user.Phone ?? string.Empty,
            Gender = user.Gender,
            Specialty = user.Specialty,
            RoleName = user.Role?.RoleName.ToString() ?? string.Empty,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };

        private static string GenerateTemporaryPassword()
        {
            const string uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string lowercase = "abcdefghijklmnopqrstuvwxyz";
            const string digits = "0123456789";
            const string specialChars = "!@#$%^&*";

            var random = new Random();
            var password = new System.Text.StringBuilder();

            password.Append(uppercase[random.Next(uppercase.Length)]);
            password.Append(lowercase[random.Next(lowercase.Length)]);
            password.Append(digits[random.Next(digits.Length)]);
            password.Append(specialChars[random.Next(specialChars.Length)]);

            const string allChars = uppercase + lowercase + digits + specialChars;
            for (int i = 0; i < 8; i++)
                password.Append(allChars[random.Next(allChars.Length)]);

            return new string(password.ToString().OrderBy(c => random.Next()).ToArray());
        }

        private static string GenerateVerifyToken()
        {
            return Convert.ToBase64String(
                System.Security.Cryptography.RandomNumberGenerator.GetBytes(64));
        }

        private static string GenerateAccountCreationEmailBody(
     string fullName,
     string email,
     string temporaryPassword,
     string verifyLink)
        {
            return $@"
        <!DOCTYPE html>
        <html>
        <head>
            <meta charset='utf-8'>
            <style>
                body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                .header {{ background-color: #007bff; color: white; padding: 20px; text-align: center; border-radius: 5px 5px 0 0; }}
                .content {{ background-color: #f9f9f9; padding: 20px; border: 1px solid #ddd; border-radius: 0 0 5px 5px; }}
                .credentials {{ background-color: #fff; padding: 15px; border-left: 4px solid #28a745; margin: 20px 0; }}
                .credentials p {{ margin: 8px 0; }}
                .verify-button {{ display: inline-block; padding: 12px 30px; background-color: #28a745; color: white; text-decoration: none; border-radius: 5px; margin: 20px 0; }}
                .footer {{ color: #666; font-size: 12px; text-align: center; margin-top: 20px; }}
            </style>
        </head>
        <body>
            <div class='container'>
                <div class='header'>
                    <h1>Chào mừng đến GodotXR!</h1>
                </div>
                <div class='content'>
                    <p>Xin chào <strong>{fullName}</strong>,</p>
                    <p>Tài khoản của bạn đã được tạo thành công. Dưới đây là thông tin đăng nhập của bạn:</p>
                    <div class='credentials'>
                        <p><strong>Email:</strong> {email}</p>
                        <p><strong>Mật khẩu tạm thời:</strong> {temporaryPassword}</p>
                    </div>
                    <p><strong>Lưu ý quan trọng:</strong></p>
                    <ul>
                        <li>Hãy đổi mật khẩu tạm thời sau khi đăng nhập lần đầu tiên</li>
                        <li>Mật khẩu của bạn là bí mật - không chia sẻ với bất kỳ ai</li>
                        <li>Mật khẩu tạm thời này sẽ hết hạn sau 24 giờ</li>
                    </ul>
                    <p>Bước tiếp theo, vui lòng xác minh email của bạn bằng cách nhấp vào nút dưới đây:</p>
                    <a href='{verifyLink}' class='verify-button'>Xác Minh Email</a>
                    <p>Hoặc sao chép và dán liên kết này vào trình duyệt:</p>
                    <p><small>{verifyLink}</small></p>
                    <p>Liên kết xác minh này sẽ hết hạn sau 24 giờ.</p>
                    <p>Nếu bạn không tạo tài khoản này, vui lòng bỏ qua email này.</p>
                    <p>Trân trọng,<br><strong>Đội ngũ GodotXR</strong></p>
                </div>
                <div class='footer'>
                    <p>&copy; 2026 GodotXR. Tất cả các quyền được bảo lưu.</p>
                </div>
            </div>
        </body>
        </html>";
        }
    }
}