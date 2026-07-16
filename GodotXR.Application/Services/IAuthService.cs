using GodotXR.Application.DTOs.Request.Auth;
using GodotXR.Application.DTOs.Response.Auth;

namespace GodotXR.Application.Services
{
    public interface IAuthService
    {
        Task<(bool Succeeded, IEnumerable<string> Errors, TokenModel? Data)> LoginAsync(LoginRequest request);
        Task<(bool Succeeded, IEnumerable<string> Errors, TokenModel? Data)> RefreshTokenAsync(RefreshTokenRequest request);
        Task<(bool Succeeded, bool NotFound, IEnumerable<string> Errors)> ForgotPasswordAsync(string email);
        Task<(bool Succeeded, bool NotFound, IEnumerable<string> Errors)> VerifyOtpAsync(string email, string otp);
        Task<(bool Succeeded, bool NotFound, IEnumerable<string> Errors)> ResetPasswordAsync(ResetPasswordRequest request);
        Task<(bool Succeeded, bool NotFound, IEnumerable<string> Errors)> ChangePasswordAsync(ChangePasswordRequest request);
        Task<(bool Succeeded, IEnumerable<string> Errors)> VerifyEmailAsync(string token); 
    }
}