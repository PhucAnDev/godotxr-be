using GodotXR.Api.Contracts;
using GodotXR.Application.DTOs.Request.Enrollment;
using GodotXR.Application.DTOs.Response;
using GodotXR.Application.DTOs.Response.Enrollment;
using GodotXR.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GodotXR.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EnrollmentsController : ControllerBase
    {
        private readonly IEnrollmentService _enrollmentService;

        public EnrollmentsController(IEnrollmentService enrollmentService)
        {
            _enrollmentService = enrollmentService;
        }

        private int GetRequesterId() =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        private string GetRequesterRole() =>
            User.FindFirstValue(ClaimTypes.Role) ?? string.Empty;

        [HttpGet]
        [Authorize(Roles = "Admin,Teacher,Parent")]
        public async Task<ActionResult> Get(
            [FromQuery] PaginationQuery query,
            [FromQuery] string? status = null,
            [FromQuery] int? classId = null,
            [FromQuery] string? learningLevel = null)
        {
            var data = await _enrollmentService.GetListEnrollmentAsync(
                query.PageNumber, query.PageSize, status, classId, learningLevel);
            return Ok(new ApiResponse<PagedResponse<EnrollmentResponse>> { Success = true, Message = "OK", Data = data });
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = "Admin,Teacher,Parent")]
        public async Task<ActionResult> GetById(int id)
        {
            if (id <= 0)
                return BadRequest(new ApiResponse<EnrollmentResponse> { Success = false, Message = "Invalid enrollment id." });

            var data = await _enrollmentService.GetEnrollmentByIdAsync(id);
            if (data is null)
                return NotFound(new ApiResponse<EnrollmentResponse> { Success = false, Message = "Enrollment not found." });

            return Ok(new ApiResponse<EnrollmentResponse> { Success = true, Message = "OK", Data = data });
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<ActionResult> Create([FromBody] CreateEnrollmentRequest request)
        {
            if (request is null)
                return BadRequest(new ApiResponse<EnrollmentResponse> { Success = false, Message = "Request body is required." });

            var (ok, errors, data) = await _enrollmentService
                .CreateEnrollmentAsync(request, GetRequesterId(), GetRequesterRole());

            if (!ok || data is null)
                return BadRequest(new ApiResponse<EnrollmentResponse> { Success = false, Message = "Tạo ghi danh thất bại.", Errors = errors.ToList() });

            return Ok(new ApiResponse<EnrollmentResponse> { Success = true, Message = "Ghi danh thành công.", Data = data });
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<ActionResult> Update(int id, [FromBody] UpdateEnrollmentRequest request)
        {
            if (id <= 0)
                return BadRequest(new ApiResponse<EnrollmentResponse> { Success = false, Message = "Invalid enrollment id." });
            if (request is null)
                return BadRequest(new ApiResponse<EnrollmentResponse> { Success = false, Message = "Request body is required." });

            var (ok, notFound, errors, data) = await _enrollmentService
                .UpdateEnrollmentAsync(id, request, GetRequesterId(), GetRequesterRole());

            if (notFound)
                return NotFound(new ApiResponse<EnrollmentResponse> { Success = false, Message = "Enrollment not found." });
            if (!ok || data is null)
                return BadRequest(new ApiResponse<EnrollmentResponse> { Success = false, Message = "Cập nhật ghi danh thất bại.", Errors = errors.ToList() });

            return Ok(new ApiResponse<EnrollmentResponse> { Success = true, Message = "Cập nhật ghi danh thành công.", Data = data });
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(int id)
        {
            if (id <= 0)
                return BadRequest(new ApiResponse<bool> { Success = false, Message = "Invalid enrollment id." });

            var (ok, notFound, errors) = await _enrollmentService.DeleteEnrollmentAsync(id);

            if (notFound)
                return NotFound(new ApiResponse<bool> { Success = false, Message = "Enrollment not found.", Data = false });
            if (!ok)
                return BadRequest(new ApiResponse<bool> { Success = false, Message = "Xóa ghi danh thất bại.", Errors = errors.ToList(), Data = false });

            return Ok(new ApiResponse<bool> { Success = true, Message = "Xóa ghi danh thành công.", Data = true });
        }

        [HttpPut("{id:int}/transfer")]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<ActionResult> Transfer(int id, [FromBody] TransferEnrollmentRequest request)
        {
            if (id <= 0)
                return BadRequest(new ApiResponse<EnrollmentResponse> { Success = false, Message = "Invalid enrollment id." });
            if (request is null)
                return BadRequest(new ApiResponse<EnrollmentResponse> { Success = false, Message = "Request body is required." });

            var (ok, notFound, errors, data) = await _enrollmentService
                .TransferEnrollmentAsync(id, request, GetRequesterId(), GetRequesterRole());

            if (notFound)
                return NotFound(new ApiResponse<EnrollmentResponse> { Success = false, Message = "Enrollment not found." });
            if (!ok || data is null)
                return BadRequest(new ApiResponse<EnrollmentResponse> { Success = false, Message = "Chuyển lớp thất bại.", Errors = errors.ToList() });

            return Ok(new ApiResponse<EnrollmentResponse> { Success = true, Message = "Chuyển lớp thành công.", Data = data });
        }

        [HttpPut("{id:int}/approve")]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<ActionResult> Approve(int id)
        {
            if (id <= 0)
                return BadRequest(new ApiResponse<EnrollmentResponse> { Success = false, Message = "Invalid enrollment id." });

            var (ok, notFound, errors, data) = await _enrollmentService
                .ApproveEnrollmentAsync(id, GetRequesterId(), GetRequesterRole());

            if (notFound)
                return NotFound(new ApiResponse<EnrollmentResponse> { Success = false, Message = "Enrollment not found." });
            if (!ok || data is null)
                return BadRequest(new ApiResponse<EnrollmentResponse> { Success = false, Message = "Duyệt ghi danh thất bại.", Errors = errors.ToList() });

            return Ok(new ApiResponse<EnrollmentResponse> { Success = true, Message = "Duyệt ghi danh thành công.", Data = data });
        }
        [HttpGet("child/{childId:int}")]
        [Authorize(Roles = "Admin,Teacher,Parent")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<EnrollmentResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> GetByChild(int childId)
        {
            if (childId <= 0)
                return BadRequest(new ApiResponse<IEnumerable<EnrollmentResponse>>
                {
                    Success = false,
                    Message = "Invalid child id."
                });

            var data = await _enrollmentService.GetEnrollmentsByChildIdAsync(childId);

            return Ok(new ApiResponse<IEnumerable<EnrollmentResponse>>
            {
                Success = true,
                Message = "OK",
                Data = data
            });
        }
    }
}