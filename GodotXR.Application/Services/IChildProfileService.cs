using GodotXR.Application.DTOs.Request.ChildProfile;
using GodotXR.Application.DTOs.Response;
using GodotXR.Application.DTOs.Response.ChildProfile;

namespace GodotXR.Application.Services
{
    public interface IChildProfileService
    {
        Task<PagedResponse<ChildProfileResponse>> GetListChildProfileAsync(int pageNumber, int pageSize);
    
        Task<ChildProfileResponse?> GetChildProfileByIdAsync(int id);

        Task<(bool Succeeded, IEnumerable<string> Errors, ChildProfileResponse? Data)>
            CreateChildProfileAsync(CreateChildProfileRequest request);

        Task<(bool Succeeded, bool NotFound, IEnumerable<string> Errors, ChildProfileResponse? Data)>
            UpdateChildProfileAsync(int id, UpdateChildProfileRequest request);

        Task<(bool Succeeded, bool NotFound, IEnumerable<string> Errors)>
            DeleteChildProfileAsync(int id);

        Task<IEnumerable<ChildProfileResponse>> GetChildProfilesByParentIdAsync(int parentId);
    }
}
