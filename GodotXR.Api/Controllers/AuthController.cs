using GodotXR.Application.DTOs.Request.Auth;
using GodotXR.Application.DTOs.Response;
using GodotXR.Application.DTOs.Response.Auth;
using GodotXR.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace GodotXR.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IDistributedCache _cache;

        public AuthController(IAuthService authService, IDistributedCache cache)
        {
            _authService = authService;
            _cache = cache;
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                var validErrors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                return BadRequest(new ApiResponse<TokenModel> { Success = false, Message = "Validation failed.", Errors = validErrors });
            }

            var (ok, errors, data) = await _authService.LoginAsync(request);

            if (!ok || data == null)
                return Unauthorized(new ApiResponse<TokenModel> { Success = false, Message = "Login failed.", Errors = errors.ToList() });

            return Ok(new ApiResponse<TokenModel> { Success = true, Message = "Login successful.", Data = data });
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            if (request == null)
                return BadRequest(new ApiResponse<TokenModel> { Success = false, Message = "Invalid request." });

            var (ok, errors, data) = await _authService.RefreshTokenAsync(request);

            if (!ok || data == null)
                return BadRequest(new ApiResponse<TokenModel> { Success = false, Message = "Refresh token failed.", Errors = errors.ToList() });

            return Ok(new ApiResponse<TokenModel> { Success = true, Message = "Token refreshed successfully.", Data = data });
        }

        [HttpPost("forgot-password")]
        public async Task<ActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            if (request is null)
                return BadRequest(new ApiResponse { Success = false, Message = "Request body is required." });

            var (ok, notFound, errors) = await _authService.ForgotPasswordAsync(request.Email);

            if (notFound)
                return NotFound(new ApiResponse { Success = false, Message = "User not found." });

            if (!ok)
                return BadRequest(new ApiResponse { Success = false, Message = "Forgot password failed.", Errors = errors.ToList() });

            return Ok(new ApiResponse { Success = true, Message = "OTP has been sent successfully." });
        }

        [HttpPost("reset-password")]
        public async Task<ActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            if (request is null)
                return BadRequest(new ApiResponse { Success = false, Message = "Request body is required." });

            var (ok, notFound, errors) = await _authService.ResetPasswordAsync(request);

            if (notFound)
                return NotFound(new ApiResponse { Success = false, Message = "User not found." });

            if (!ok)
                return BadRequest(new ApiResponse { Success = false, Message = "Reset password failed.", Errors = errors.ToList() });

            return Ok(new ApiResponse { Success = true, Message = "Password has been reset successfully." });
        }

        [HttpPost("change-password")]
        public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            if (request is null)
                return BadRequest(new ApiResponse { Success = false, Message = "Request body is required." });

            var (ok, notFound, errors) = await _authService.ChangePasswordAsync(request);

            if (notFound)
                return NotFound(new ApiResponse { Success = false, Message = "User not found." });

            if (!ok)
                return BadRequest(new ApiResponse { Success = false, Message = "Change password failed.", Errors = errors.ToList() });

            return Ok(new ApiResponse { Success = true, Message = "Password has been changed successfully." });
        }

        // ✅ Endpoint mới
        [HttpGet("verify-email")]
        public async Task<IActionResult> VerifyEmail([FromQuery] string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return BadRequest(new ApiResponse { Success = false, Message = "Token is required." });

            var (succeeded, errors) = await _authService.VerifyEmailAsync(token);

            if (!succeeded)
                return BadRequest(new ApiResponse { Success = false, Message = errors.FirstOrDefault() ?? "Xác minh thất bại." });

            return Ok(new ApiResponse { Success = true, Message = "Email xác minh thành công. Bạn có thể đăng nhập." });
        }
    }
}