using GodotXR.Api.Contracts;
using GodotXR.Api.Extensions;
using GodotXR.Application.DTOs.Request.Analyze;
using GodotXR.Application.DTOs.Response;
using GodotXR.Application.DTOs.Response.Analyze;
using GodotXR.Application.Services;
using GodotXR.Domain.IUnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GodotXR.Api.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AnalyzeController : ControllerBase
    {
        private readonly IAnalyzeService _analyzeService;
        private readonly IUnitOfWork _unitOfWork;
        //Test CI/CD
        public AnalyzeController(IAnalyzeService analyzeService, IUnitOfWork unitOfWork)
        {
            _analyzeService = analyzeService;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Teacher")]
        [ProducesResponseType(typeof(ApiResponse<PagedResponse<AnalyzeResponse>>), StatusCodes.Status200OK)]
        public async Task<ActionResult> Get([FromQuery] PaginationQuery query)
        {
            var data = await _analyzeService.GetListAnalyzeAsync(
                query.PageNumber,
                query.PageSize);

            return Ok(new ApiResponse<PagedResponse<AnalyzeResponse>>
            {
                Success = true,
                Message = "OK",
                Data = data,
            });
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = "Admin,Teacher,Parent")]
        [ProducesResponseType(typeof(ApiResponse<AnalyzeResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<AnalyzeResponse>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetById(int id)
        {
            if (id <= 0)
                return BadRequest(new ApiResponse<AnalyzeResponse>
                {
                    Success = false,
                    Message = "Invalid analyze id."
                });

            var data = await _analyzeService.GetAnalyzeByIdAsync(id);

            if (data == null)
                return NotFound(new ApiResponse<AnalyzeResponse>
                {
                    Success = false,
                    Message = "Analyze not found."
                });

            var currentUserId = User.GetUserId();

            if (User.IsInRole("Parent"))
            {
                var child = await _unitOfWork.ChildProfileRepository.GetByIdAsync(data.ChildId);
                if (child == null || child.UserId != currentUserId)
                    return Forbid();
            }

            if (User.IsInRole("Teacher"))
            {
                var hasAccess = await _unitOfWork.EnrollmentRepository
                    .HasTeacherAccessToChildAsync(currentUserId, data.ChildId);
                if (!hasAccess)
                    return Forbid();
            }

            return Ok(new ApiResponse<AnalyzeResponse>
            {
                Success = true,
                Message = "OK",
                Data = data
            });
        }

        [HttpGet("child/{childId:int}")]
        [Authorize(Roles = "Admin,Teacher,Parent")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<AnalyzeResponse>>), StatusCodes.Status200OK)]
        public async Task<ActionResult> GetByChildId(int childId)
        {
            if (childId <= 0)
                return BadRequest(new ApiResponse<IEnumerable<AnalyzeResponse>>
                {
                    Success = false,
                    Message = "Invalid child id."
                });

            var currentUserId = User.GetUserId();

            if (User.IsInRole("Parent"))
            {
                var child = await _unitOfWork.ChildProfileRepository.GetByIdAsync(childId);
                if (child == null || child.UserId != currentUserId)
                    return Forbid();
            }

            if (User.IsInRole("Teacher"))
            {
                var hasAccess = await _unitOfWork.EnrollmentRepository
                    .HasTeacherAccessToChildAsync(currentUserId, childId);
                if (!hasAccess)
                    return Forbid();
            }

            var data = await _analyzeService.GetAnalyzesByChildIdAsync(childId);
            return Ok(new ApiResponse<IEnumerable<AnalyzeResponse>>
            {
                Success = true,
                Message = "OK",
                Data = data
            });
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Teacher")]
        [ProducesResponseType(typeof(ApiResponse<AnalyzeResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<AnalyzeResponse>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Create(
            [FromBody] CreateAnalyzeRequest request)
        {
            var (ok, errors, data) = await _analyzeService.CreateAnalyzeAsync(request);

            if (!ok)
            {
                return BadRequest(new ApiResponse<AnalyzeResponse>
                {
                    Success = false,
                    Message = "Create analyze failed.",
                    Errors = errors.ToList()
                });
            }

            return Ok(new ApiResponse<AnalyzeResponse>
            {
                Success = true,
                Message = "Analyze created.",
                Data = data
            });
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin,Teacher")]
        [ProducesResponseType(typeof(ApiResponse<AnalyzeResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<AnalyzeResponse>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<AnalyzeResponse>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Update(int id, [FromBody] UpdateAnalyzeRequest request)
        {
            if (id <= 0)
                return BadRequest(new ApiResponse<AnalyzeResponse>
                {
                    Success = false,
                    Message = "Invalid analyze id."
                });

            var (ok, notFound, errors, data) = await _analyzeService.UpdateAnalyzeAsync(id, request);

            if (notFound)
                return NotFound(new ApiResponse<AnalyzeResponse>
                {
                    Success = false,
                    Message = "Analyze not found."
                });

            if (!ok || data == null)
                return BadRequest(new ApiResponse<AnalyzeResponse>
                {
                    Success = false,
                    Message = "Update analyze failed.",
                    Errors = errors.ToList()
                });

            return Ok(new ApiResponse<AnalyzeResponse>
            {
                Success = true,
                Message = "Analyze updated.",
                Data = data
            });
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
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
                    Message = "Invalid analyze id."
                });
            }

            var (ok, notFound, errors) = await _analyzeService.DeleteAnalyzeAsync(id);

            if (notFound)
            {
                return NotFound(new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Analyze not found.",
                    Data = false
                });
            }

            if (!ok)
            {
                return BadRequest(new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Delete analyze failed.",
                    Errors = errors.ToList(),
                    Data = false
                });
            }

            return Ok(new ApiResponse<bool>
            {
                Success = true,
                Message = "Analyze deleted.",
                Data = true
            });
        }
    }
}
