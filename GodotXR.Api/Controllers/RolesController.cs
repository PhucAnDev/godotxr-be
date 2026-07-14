using GodotXR.Api.Contracts;
using GodotXR.Application.DTOs.Request.Role;
using GodotXR.Application.DTOs.Response;
using GodotXR.Application.DTOs.Response.Role;
using GodotXR.Application.DTOs.Response.User;
using GodotXR.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace GodotXR.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RolesController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PagedResponse<RoleResponse>>), StatusCodes.Status200OK)]
        public async Task<ActionResult> Get([FromQuery] PaginationQuery query)
        {
            var data = await _roleService.GetListRoleAsync(
                query.PageNumber,
                query.PageSize);

            return Ok(new ApiResponse<PagedResponse<RoleResponse>>
            {
                Success = true,
                Message = "OK",
                Data = data,
            });
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<RoleResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<RoleResponse>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<RoleResponse>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetById(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new ApiResponse<RoleResponse>
                {
                    Success = false,
                    Message = "Invalid role id."
                });
            }

            var data = await _roleService.GetRoleByIdAsync(id);

            if (data == null)
            {
                return NotFound(new ApiResponse<RoleResponse>
                {
                    Success = false,
                    Message = "Role not found."
                });
            }

            return Ok(new ApiResponse<RoleResponse>
            {
                Success = true,
                Message = "OK",
                Data = data
            });
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<RoleResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<RoleResponse>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<RoleResponse>>> Create([FromBody] CreateRoleRequest request)
        {
            var (ok, errors, data) = await _roleService.CreateRoleAsync(request);

            if (!ok || data == null)
            {
                return BadRequest(new ApiResponse<RoleResponse>
                {
                    Success = false,
                    Message = "Create role failed.",
                    Errors = errors.ToList()
                });
            }

            return Ok(new ApiResponse<RoleResponse>
            {
                Success = true,
                Message = "Role created.",
                Data = data
            });
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<RoleResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<RoleResponse>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<RoleResponse>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<RoleResponse>>> Update(
            int id, [FromBody] UpdateRoleRequest request)
        {
            if (id <= 0)
            {
                return BadRequest(new ApiResponse<UserResponse>
                {
                    Success = false,
                    Message = "Invalid role id."
                });
            }

            var (ok, notFound, errors, data) = await _roleService.UpdateRoleAsync(id, request);

            if (notFound)
            {
                return NotFound(new ApiResponse<RoleResponse>
                {
                    Success = false,
                    Message = "Role not found."
                });
            }

            if (!ok || data == null)
            {
                return BadRequest(new ApiResponse<RoleResponse>
                {
                    Success = false,
                    Message = "Update role failed.",
                    Errors = errors.ToList()
                });
            }

            return Ok(new ApiResponse<RoleResponse>
            {
                Success = true,
                Message = "Role updated.",
                Data = data
            });
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse>> Delete(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new ApiResponse<UserResponse>
                {
                    Success = false,
                    Message = "Invalid role id."
                });
            }

            var (ok, notFound, errors) = await _roleService.DeleteRoleAsync(id);

            if (notFound)
            {
                return NotFound(new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Role not found.",
                    Data = false
                });
            }

            if (!ok)
            {
                return BadRequest(new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Delete role failed.",
                    Errors = errors.ToList(),
                    Data = false
                });
            }

            return Ok(new ApiResponse<bool>
            {
                Success = true,
                Message = "Role deleted.",
                Data = true
            });
        }
    }
}