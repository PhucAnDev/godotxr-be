using GodotXR.Api.Contracts;
using GodotXR.Api.Extensions;
using GodotXR.Application.DTOs.Request.User;
using GodotXR.Application.DTOs.Response;
using GodotXR.Application.DTOs.Response.User;
using GodotXR.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GodotXR.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<ActionResult> Get([FromQuery] PaginationQuery query)
        {
            var data = await _userService.GetListUserAsync(query.PageNumber, query.PageSize);
            return Ok(new ApiResponse<PagedResponse<UserResponse>> { Success = true, Message = "OK", Data = data });
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = "Admin,Teacher,Parent")]
        public async Task<ActionResult> GetById(int id)
        {
            if (id <= 0)
                return BadRequest(new ApiResponse<UserResponse> { Success = false, Message = "Invalid user id." });

            var data = await _userService.GetUserByIdAsync(id);

            if (data is null)
                return NotFound(new ApiResponse<UserResponse> { Success = false, Message = "User not found." });

            return Ok(new ApiResponse<UserResponse> { Success = true, Message = "OK", Data = data });
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Create([FromBody] CreateUserRequest request)
        {
            if (request is null)
                return BadRequest(new ApiResponse<UserResponse> { Success = false, Message = "Request body is required." });

            var (ok, errors, data) = await _userService.CreateUserAsync(request);

            if (!ok || data is null)
                return BadRequest(new ApiResponse<UserResponse> { Success = false, Message = "Create user failed.", Errors = errors.ToList() });

            return Ok(new ApiResponse<UserResponse> { Success = true, Message = "User created.", Data = data });
        }

        [HttpPost("create-account")]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<ActionResult> CreateAccount([FromBody] CreateAccountRequest request)
        {
            if (!ModelState.IsValid)
            {
                var validErrors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                return BadRequest(new ApiResponse<CreateAccountResponse> { Success = false, Message = "Validation failed.", Errors = validErrors });
            }

            var (ok, errors, data) = await _userService.CreateAccountAsync(request);

            if (!ok || data is null)
                return BadRequest(new ApiResponse<CreateAccountResponse> { Success = false, Message = "Tạo tài khoản thất bại.", Errors = errors.ToList() });

            return Ok(new ApiResponse<CreateAccountResponse> { Success = true, Message = "Tạo tài khoản thành công.", Data = data });
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Update(int id, [FromBody] UpdateUserRequest request)
        {
            if (id <= 0)
                return BadRequest(new ApiResponse<UserResponse> { Success = false, Message = "Invalid user id." });

            if (request is null)
                return BadRequest(new ApiResponse<UserResponse> { Success = false, Message = "Request body is required." });

            var (ok, notFound, errors, data) = await _userService.UpdateUserAsync(id, request);

            if (notFound)
                return NotFound(new ApiResponse<UserResponse> { Success = false, Message = "User not found." });

            if (!ok || data is null)
                return BadRequest(new ApiResponse<UserResponse> { Success = false, Message = "Update user failed.", Errors = errors.ToList() });

            return Ok(new ApiResponse<UserResponse> { Success = true, Message = "User updated.", Data = data });
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(int id)
        {
            if (id <= 0)
                return BadRequest(new ApiResponse<UserResponse> { Success = false, Message = "Invalid user id." });

            var (ok, notFound, errors) = await _userService.DeleteUserAsync(id);

            if (notFound)
                return NotFound(new ApiResponse<bool> { Success = false, Message = "User not found.", Data = false });

            if (!ok)
                return BadRequest(new ApiResponse<bool> { Success = false, Message = "Delete user failed.", Errors = errors.ToList(), Data = false });

            return Ok(new ApiResponse<bool> { Success = true, Message = "User deleted.", Data = true });
        }

        [Authorize]
        [HttpGet("children-profiles")]
        [ProducesResponseType(typeof(ApiResponse<UserWithChildrenProfileResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<UserWithChildrenProfileResponse>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<UserWithChildrenProfileResponse>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> GetCurrentUserWithChildrenProfiles()
        {
            var userId = User.GetUserId();

            var (succeeded, notFound, errors, data) =
                await _userService.GetCurrentUserWithChildrenProfilesAsync(userId);

            if (notFound)
            {
                return NotFound(new ApiResponse<UserWithChildrenProfileResponse>
                {
                    Success = false,
                    Message = "User not found."
                });
            }

            if (!succeeded)
            {
                return BadRequest(new ApiResponse<UserWithChildrenProfileResponse>
                {
                    Success = false,
                    Errors = errors.ToList()
                });
            }

            return Ok(new ApiResponse<UserWithChildrenProfileResponse>
            {
                Success = true,
                Message = "OK",
                Data = data
            });
        }
    }
}