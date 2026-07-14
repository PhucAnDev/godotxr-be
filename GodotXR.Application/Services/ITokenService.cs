using System.Security.Claims;
using GodotXR.Domain.Entities;

namespace GodotXR.Application.Services
{
    public interface ITokenService
    {
        string GenerateAccessToken(User user);
        string GenerateRefreshToken();
        ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
    }
}
