namespace GodotXR.Application.Services
{
    public interface IPasswordHasherService
    {
        string Hash(string plainTextPassword);
        bool Verify(string plainTextPassword, string hashedPassword);
    }
}
