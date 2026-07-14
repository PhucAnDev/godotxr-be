using GodotXR.Api.Contracts;
using GodotXR.Api.Extensions;
using GodotXR.Application.DTOs.Request.ChildProfile;
using GodotXR.Application.DTOs.Response;
using GodotXR.Application.DTOs.Response.ChildProfile;
using GodotXR.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GodotXR.Api.Controllers
{
    [ApiController]
    [Route("api/child-profiles")]
    [Authorize(Roles = "Admin,Teacher,Parent")]
    public class ChildProfilesController : ControllerBase
    {
        private readonly IChildProfileService _childProfileService;

        public ChildProfilesController(IChildProfileService childProfileService)
        {
            _childProfileService = childProfileService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PagedResponse<ChildProfileResponse>>), StatusCodes.Status200OK)]
        public async Task<ActionResult> Get([FromQuery] PaginationQuery query)
        {
            var data = await _childProfileService.GetListChildProfileAsync(
                query.PageNumber,
                query.PageSize);

            return Ok(new ApiResponse<PagedResponse<ChildProfileResponse>>
            {
                Success = true,
                Message = "OK",
                Data = data,
            });
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<ChildProfileResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<ChildProfileResponse>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetById(int id)
        {
            if (id <= 0)
                return BadRequest(new ApiResponse<ChildProfileResponse>
                {
                    Success = false,
                    Message = "Invalid child profile id."
                });

            var data = await _childProfileService.GetChildProfileByIdAsync(id);

            if (data == null)
                return NotFound(new ApiResponse<ChildProfileResponse>
                {
                    Success = false,
                    Message = "Child profile not found."
                });

            return Ok(new ApiResponse<ChildProfileResponse>
            {
                Success = true,
                Message = "OK",
                Data = data
            });
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<ChildProfileResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<ChildProfileResponse>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Create(
            [FromBody] CreateChildProfileRequest request)
        {
            var (ok, errors, data) = await _childProfileService.CreateChildProfileAsync(request);

            if (!ok)
            {
                return BadRequest(new ApiResponse<ChildProfileResponse>
                {
                    Success = false,
                    Message = "Create child profile failed.",
                    Errors = errors.ToList()
                });
            }

            return Ok(new ApiResponse<ChildProfileResponse>
            {
                Success = true,
                Message = "Child profile created.",
                Data = data
            });
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<ChildProfileResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<ChildProfileResponse>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<ChildProfileResponse>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Update(int id, [FromBody] UpdateChildProfileRequest request)
        {
            if (id <= 0)
                return BadRequest(new ApiResponse<ChildProfileResponse>
                {
                    Success = false,
                    Message = "Invalid child profile id."
                });

            var (ok, notFound, errors, data) = await _childProfileService.UpdateChildProfileAsync(id, request);

            if (notFound)
                return NotFound(new ApiResponse<ChildProfileResponse>
                {
                    Success = false,
                    Message = "Child profile not found."
                });

            if (!ok || data == null)
                return BadRequest(new ApiResponse<ChildProfileResponse>
                {
                    Success = false,
                    Message = "Update child profile failed.",
                    Errors = errors.ToList()
                });

            return Ok(new ApiResponse<ChildProfileResponse>
            {
                Success = true,
                Message = "Child profile updated.",
                Data = data
            });
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Invalid child profile id."
                });
            }

            var (ok, notFound, errors) = await _childProfileService.DeleteChildProfileAsync(id);

            if (notFound)
            {
                return NotFound(new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Child profile not found.",
                    Data = false
                });
            }

            if (!ok)
            {
                return BadRequest(new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Delete child profile failed.",
                    Errors = errors.ToList(),
                    Data = false
                });
            }

            return Ok(new ApiResponse<bool>
            {
                Success = true,
                Message = "Child profile deleted.",
                Data = true
            });
        }

        [HttpGet("my-children")]
        [Authorize(Roles = "Parent")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ChildProfileResponse>>), StatusCodes.Status200OK)]
        public async Task<ActionResult> GetMyChildProfiles()
        {
            var parentId = User.GetUserId();

            var data = await _childProfileService.GetChildProfilesByParentIdAsync(parentId);

            return Ok(new ApiResponse<IEnumerable<ChildProfileResponse>>
            {
                Success = true,
                Message = "OK",
                Data = data
            });
        }
    }
}
