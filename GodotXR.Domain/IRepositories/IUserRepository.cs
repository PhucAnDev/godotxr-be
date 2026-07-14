using GodotXR.Domain.Entities;

namespace GodotXR.Domain.IRepositories
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User?> GetByIdWithChildProfilesAsync(int id);
    }
}
