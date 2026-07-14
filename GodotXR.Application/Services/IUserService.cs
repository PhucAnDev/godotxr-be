using GodotXR.Application.DTOs.Request.User;
using GodotXR.Application.DTOs.Response;
using GodotXR.Application.DTOs.Response.User;

namespace GodotXR.Application.Services
{
    public interface IUserService
    {
        Task<PagedResponse<UserResponse>> GetListUserAsync(int pageNumber, int pageSize);

        Task<UserResponse?> GetUserByIdAsync(int id);

        Task<(bool Succeeded, IEnumerable<string> Errors, UserResponse? Data)> CreateUserAsync(CreateUserRequest request);

        Task<(bool Succeeded, IEnumerable<string> Errors, CreateAccountResponse? Data)> CreateAccountAsync(CreateAccountRequest request);

        Task<(bool Succeeded, bool NotFound, IEnumerable<string> Errors, UserResponse? Data)> UpdateUserAsync(int id, UpdateUserRequest request);

        Task<(bool Succeeded, bool NotFound, IEnumerable<string> Errors)> DeleteUserAsync(int id);

        Task<(bool Succeeded, bool NotFound, IEnumerable<string> Errors, UserWithChildrenProfileResponse? Data)> 
            GetCurrentUserWithChildrenProfilesAsync(int userId);
    }
}
