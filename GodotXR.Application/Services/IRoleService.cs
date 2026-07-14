using GodotXR.Application.DTOs.Request.Role;
using GodotXR.Application.DTOs.Response;
using GodotXR.Application.DTOs.Response.Role;

namespace GodotXR.Application.Services
{
    public interface IRoleService
    {
        Task<PagedResponse<RoleResponse>> GetListRoleAsync(int pageNumber, int pageSize);

        Task<RoleResponse?> GetRoleByIdAsync(int id);

        Task<(bool Succeeded, IEnumerable<string> Errors, RoleResponse? Data)>
            CreateRoleAsync(CreateRoleRequest request);

        Task<(bool Succeeded, bool NotFound, IEnumerable<string> Errors, RoleResponse? Data)>
            UpdateRoleAsync(int id, UpdateRoleRequest request);

        Task<(bool Succeeded, bool NotFound, IEnumerable<string> Errors)>
            DeleteRoleAsync(int id);
    }
}