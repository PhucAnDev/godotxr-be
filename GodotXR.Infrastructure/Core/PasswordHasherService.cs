using GodotXR.Application.Services;

namespace GodotXR.Infrastructure.Core
{
    public class PasswordHasherService : IPasswordHasherService
    {
        public string Hash(string plainTextPassword)
        {
            return BCrypt.Net.BCrypt.HashPassword(plainTextPassword);
        }

        public bool Verify(string plainTextPassword, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(plainTextPassword, hashedPassword);
        }
    }
}
