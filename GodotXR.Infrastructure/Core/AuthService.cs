using GodotXR.Application.DTOs.Request.Auth;
using GodotXR.Application.DTOs.Response.Auth;
using GodotXR.Application.Services;
using GodotXR.Domain.Entities;
using GodotXR.Domain.Enums;
using GodotXR.Domain.IUnitOfWork;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;

namespace GodotXR.Infrastructure.Core
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenService _tokenService;
        private readonly IDistributedCache _cache;
        private readonly IMailService _mailService;
        private readonly IPasswordHasherService _passwordHasherService;
        private readonly IConfiguration _configuration;

        public AuthService(
            IUnitOfWork unitOfWork,
            ITokenService tokenService,
            IConfiguration configuration,
            IDistributedCache cache,
            IMailService mailService,
            IPasswordHasherService passwordHasherService)
        {
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
            _configuration = configuration;
            _cache = cache;
            _mailService = mailService;
            _passwordHasherService = passwordHasherService;
        }

        public async Task<(bool Succeeded, IEnumerable<string> Errors, RegisterResponse? Data)> RegisterAsync(RegisterRequest request)
        {
            var emailExists = await _unitOfWork.UserRepository.ExistsAsync(
                u => u.Email == request.Email && !u.IsDeleted);

            if (emailExists)
                return (false, new[] { $"Email '{request.Email}' đã tồn tại." }, null);

            var parentRole = await _unitOfWork.RoleRepository.GetFirstOrDefaultAsync(
                r => r.RoleName == UserRole.Parent && r.IsActive);

            if (parentRole == null)
                return (false, new[] { "Role 'Parent' không tồn tại hoặc không hoạt động." }, null);

            var verifyToken = Convert.ToBase64String(
                System.Security.Cryptography.RandomNumberGenerator.GetBytes(64));

            var user = new User
            {
                PasswordHash = _passwordHasherService.Hash(request.Password),
                FullName = request.FullName,
                Email = request.Email,
                Phone = request.Phone,
                RoleId = parentRole.Id,
                IsActive = false,
                IsEmailVerified = false,
                MustChangePassword = false,
                VerifyToken = verifyToken,
                VerifyTokenExpiry = DateTime.UtcNow.AddHours(24)
            };

            await _unitOfWork.UserRepository.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            var frontendUrl = _configuration["App:FrontendBaseUrl"] ?? "http://localhost:3000";
            var verifyLink = $"{frontendUrl}/verify-email?token={Uri.EscapeDataString(verifyToken)}";

            var subject = "Xác nhận đăng ký tài khoản - GodotXR";
            var body = $@"
                <div style='font-family: Arial, Helvetica, sans-serif; max-width: 600px; margin: 0 auto; background-color: #ffffff; border: 1px solid #e5e5e5; border-radius: 12px; overflow: hidden;'>
                    <div style='background: linear-gradient(135deg, #4CAF50, #2E7D32); padding: 24px; text-align: center; color: white;'>
                        <h1 style='margin: 0;'>GodotXR</h1>
                        <p style='margin-top: 8px;'>Xác nhận đăng ký tài khoản</p>
                    </div>
                    <div style='padding: 32px;'>
                        <h2 style='color: #333;'>Xin chào {request.FullName},</h2>
                        <p style='color: #555; line-height: 1.8;'>Cảm ơn bạn đã đăng ký tài khoản GodotXR. Vui lòng xác nhận email để kích hoạt tài khoản.</p>
                        <div style='text-align: center; margin: 30px 0;'>
                            <a href='{verifyLink}' style='display: inline-block; padding: 12px 30px; background-color: #28a745; color: white; text-decoration: none; border-radius: 5px;'>Xác Nhận Email</a>
                        </div>
                        <p style='color: #555;'>Hoặc sao chép liên kết sau vào trình duyệt:</p>
                        <p style='color: #555; word-break: break-all;'>{verifyLink}</p>
                        <p style='color: #555;'>Liên kết có hiệu lực trong <strong>24 giờ</strong>.</p>
                    </div>
                    <div style='background-color: #f8f8f8; text-align: center; padding: 16px; font-size: 12px; color: #888;'>
                        © {DateTime.Now.Year} GodotXR.
                    </div>
                </div>";

            try
            {
                await _mailService.SendEmailAsync(request.Email, subject, body);
            }
            catch (Exception ex)
            {
                return (false, new[] { $"Gửi email xác nhận thất bại: {ex.Message}" }, null);
            }

            return (true, Enumerable.Empty<string>(), new RegisterResponse
            {
                UserId = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                RoleName = parentRole.RoleName.ToString(),
                Message = "Đăng ký thành công. Vui lòng kiểm tra email để xác nhận tài khoản."
            });
        }

        public async Task<(bool Succeeded, IEnumerable<string> Errors, TokenModel? Data)> LoginAsync(LoginRequest request)
        {
            var user = await _unitOfWork.UserRepository.GetFirstOrDefaultAsync(
                u => u.Email == request.Email,
                includeProperties: "Role");

            if (user == null)
                return (false, new[] { "Invalid email or password." }, null);

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                return (false, new[] { "Invalid email or password." }, null);

            if (!user.IsEmailVerified)
                return (false, new[] { "Email chưa được xác minh. Vui lòng kiểm tra hộp thư." }, null);

            var accessToken = _tokenService.GenerateAccessToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken();

            var cacheKey = $"refreshToken:{user.Email}";
            await _cache.SetStringAsync(cacheKey, refreshToken, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(7)
            });

            var token = new TokenModel
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                User = new UserAuthInfo
                {
                    Id = user.Id,
                    Email = user.Email,
                    FullName = user.FullName,
                    Phone = user.Phone ?? string.Empty,
                    RoleName = user.Role.RoleName.ToString(),
                    IsActive = user.IsActive,
                    MustChangePassword = user.MustChangePassword
                }
            };

            return (true, Enumerable.Empty<string>(), token);
        }

        public async Task<(bool Succeeded, IEnumerable<string> Errors, TokenModel? Data)> RefreshTokenAsync(RefreshTokenRequest request)
        {
            try
            {
                var principal = _tokenService.GetPrincipalFromExpiredToken(request.AccessToken);

                if (principal == null)
                    return (false, new[] { "Invalid access token." }, null);

                var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (!int.TryParse(userIdClaim, out var userId))
                    return (false, new[] { "Invalid token." }, null);

                var user = await _unitOfWork.UserRepository.GetFirstOrDefaultAsync(
                    u => u.Id == userId,
                    includeProperties: "Role");

                if (user == null)
                    return (false, new[] { "User not found." }, null);

                var cacheKey = $"refreshToken:{user.Email}";
                var savedRefreshToken = await _cache.GetStringAsync(cacheKey);

                if (savedRefreshToken != request.RefreshToken)
                    return (false, new[] { "Invalid refresh token." }, null);

                var newAccessToken = _tokenService.GenerateAccessToken(user);
                var newRefreshToken = _tokenService.GenerateRefreshToken();

                await _cache.SetStringAsync(cacheKey, newRefreshToken, new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(7)
                });

                return (true, Enumerable.Empty<string>(), new TokenModel
                {
                    AccessToken = newAccessToken,
                    RefreshToken = newRefreshToken,
                    User = new UserAuthInfo
                    {
                        Id = user.Id,
                        Email = user.Email,
                        FullName = user.FullName,
                        Phone = user.Phone ?? string.Empty,
                        RoleName = user.Role.RoleName.ToString(),
                        IsActive = user.IsActive,
                        MustChangePassword = user.MustChangePassword
                    }
                });
            }
            catch (Exception ex)
            {
                return (false, new[] { $"Refresh token failed: {ex.Message}" }, null);
            }
        }

        public async Task<(bool Succeeded, bool NotFound, IEnumerable<string> Errors)> ForgotPasswordAsync(string email)
        {
            var user = await _unitOfWork.UserRepository
                .GetFirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
                return (false, true, Enumerable.Empty<string>());

            var otpCode = Random.Shared.Next(100000, 999999).ToString();
            var cacheKey = $"otp:{email}";

            try
            {
                await _cache.SetStringAsync(cacheKey, otpCode, new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                });

                var savedOtp = await _cache.GetStringAsync(cacheKey);
                if (savedOtp != otpCode)
                    return (false, false, new[] { "Failed to save OTP to cache." });
            }
            catch (Exception ex)
            {
                return (false, false, new[] { $"Redis error: {ex.Message}" });
            }

            var subject = "Mã OTP Đặt Lại Mật Khẩu - GodotXR";
            var body = $@"
                <div style='font-family: Arial, Helvetica, sans-serif; max-width: 600px; margin: 0 auto; background-color: #ffffff; border: 1px solid #e5e5e5; border-radius: 12px; overflow: hidden;'>
                    <div style='background: linear-gradient(135deg, #4CAF50, #2E7D32); padding: 24px; text-align: center; color: white;'>
                        <h1 style='margin: 0;'>GodotXR</h1>
                        <p style='margin-top: 8px;'>Xác thực yêu cầu đặt lại mật khẩu</p>
                    </div>
                    <div style='padding: 32px;'>
                        <h2 style='color: #333;'>Xin chào {user.FullName},</h2>
                        <p style='color: #555; line-height: 1.8;'>Chúng tôi đã nhận được yêu cầu đặt lại mật khẩu cho tài khoản GodotXR của bạn.</p>
                        <div style='text-align: center; margin: 30px 0;'>
                            <div style='display: inline-block; background-color: #f5f5f5; border: 2px dashed #4CAF50; border-radius: 10px; padding: 16px 32px; font-size: 32px; font-weight: bold; letter-spacing: 8px; color: #2E7D32;'>
                                {otpCode}
                            </div>
                        </div>
                        <p style='color: #555;'>Mã có hiệu lực trong <strong>5 phút</strong>. Không chia sẻ mã này với bất kỳ ai.</p>
                    </div>
                    <div style='background-color: #f8f8f8; text-align: center; padding: 16px; font-size: 12px; color: #888;'>
                        © {DateTime.Now.Year} GodotXR.
                    </div>
                </div>";

            try
            {
                await _mailService.SendEmailAsync(user.Email, subject, body);
                return (true, false, Enumerable.Empty<string>());
            }
            catch (Exception ex)
            {
                return (false, false, new[] { $"Failed to send email: {ex.Message}" });
            }
        }

        public async Task<(bool Succeeded, bool NotFound, IEnumerable<string> Errors)> VerifyOtpAsync(string email, string otp)
        {
            var user = await _unitOfWork.UserRepository
                .GetFirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
                return (false, true, Enumerable.Empty<string>());

            var cacheKey = $"otp:{email}";
            var savedOtp = await _cache.GetStringAsync(cacheKey);

            if (string.IsNullOrEmpty(savedOtp?.Trim()) || savedOtp.Trim() != otp?.Trim())
                return (false, false, new[] { "Mã OTP không hợp lệ hoặc đã hết hạn." });

            return (true, false, Enumerable.Empty<string>());
        }

        public async Task<(bool Succeeded, bool NotFound, IEnumerable<string> Errors)> ResetPasswordAsync(ResetPasswordRequest request)
        {
            var user = await _unitOfWork.UserRepository
                .GetFirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null)
                return (false, true, Enumerable.Empty<string>());

            var cacheKey = $"otp:{request.Email}";
            var savedOtp = await _cache.GetStringAsync(cacheKey);

            if (string.IsNullOrEmpty(savedOtp?.Trim()) || savedOtp.Trim() != request.Otp?.Trim())
                return (false, false, new[] { "Invalid or expired OTP code." });

            await _cache.RemoveAsync(cacheKey);

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            user.UpdatedAt = DateTime.UtcNow;

            var affectedRows = await _unitOfWork.SaveChangesAsync();
            if (affectedRows <= 0)
                return (false, false, new[] { "Failed to reset password." });

            return (true, false, Enumerable.Empty<string>());
        }

        public async Task<(bool Succeeded, bool NotFound, IEnumerable<string> Errors)> ChangePasswordAsync(ChangePasswordRequest request)
        {
            var user = await _unitOfWork.UserRepository
                .GetFirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null)
                return (false, true, Enumerable.Empty<string>());

            if (string.IsNullOrWhiteSpace(request.Password) ||
                string.IsNullOrWhiteSpace(request.NewPassword) ||
                string.IsNullOrWhiteSpace(request.ConfirmPassword))
                return (false, false, new[] { "All password fields are required." });

            if (request.NewPassword != request.ConfirmPassword)
                return (false, false, new[] { "New password and confirmation password do not match." });

            if (request.Password == request.NewPassword)
                return (false, false, new[] { "New password must be different from the current password." });

            if (!_passwordHasherService.Verify(request.Password, user.PasswordHash))
                return (false, false, new[] { "Current password is incorrect." });

            user.PasswordHash = _passwordHasherService.Hash(request.NewPassword);
            user.MustChangePassword = false;
            user.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync();
            return (true, false, Enumerable.Empty<string>());
        }

        public async Task<(bool Succeeded, IEnumerable<string> Errors)> VerifyEmailAsync(string token)
        {
            if (!string.IsNullOrEmpty(token) && token.Contains(" "))
            {
                token = token.Replace(" ", "+");
            }

            var user = await _unitOfWork.UserRepository
                .GetFirstOrDefaultAsync(u => u.VerifyToken == token);

            if (user == null || user.VerifyTokenExpiry < DateTime.UtcNow)
                return (false, new[] { "Token không hợp lệ hoặc đã hết hạn." });

            user.IsEmailVerified = true;
            user.IsActive = true;
            user.VerifyToken = null;
            user.VerifyTokenExpiry = null;
            user.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync();
            return (true, Enumerable.Empty<string>());
        }
    }
}