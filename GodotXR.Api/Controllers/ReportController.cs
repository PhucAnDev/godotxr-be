using GodotXR.Api.Contracts;
using GodotXR.Api.Extensions;
using GodotXR.Application.DTOs.Request.Report;
using GodotXR.Application.DTOs.Response;
using GodotXR.Application.DTOs.Response.Report;
using GodotXR.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GodotXR.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Teacher,Parent")]
        [ProducesResponseType(typeof(ApiResponse<PagedResponse<ReportResponse>>), StatusCodes.Status200OK)]
        public async Task<ActionResult> Get([FromQuery] PaginationQuery query)
        {
            var userId = User.GetUserId();
            var role = User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;

            var data = await _reportService.GetListReportAsync(query.PageNumber, query.PageSize, userId, role);
            return Ok(new ApiResponse<PagedResponse<ReportResponse>> { Success = true, Message = "OK", Data = data });
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = "Admin,Teacher,Parent")]
        [ProducesResponseType(typeof(ApiResponse<ReportResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetById(int id)
        {
            if (id <= 0)
                return BadRequest(new ApiResponse<ReportResponse> { Success = false, Message = "Invalid report id." });

            var data = await _reportService.GetReportByIdAsync(id);
            if (data == null)
                return NotFound(new ApiResponse<ReportResponse> { Success = false, Message = "Report not found." });

            return Ok(new ApiResponse<ReportResponse> { Success = true, Message = "OK", Data = data });
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Teacher,Parent")]
        [ProducesResponseType(typeof(ApiResponse<ReportResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<ReportResponse>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Create([FromBody] CreateReportRequest request)
        {
            var generatedBy = User.GetUserId();
            var (ok, errors, data) = await _reportService.CreateReportAsync(generatedBy, request);

            if (!ok)
                return BadRequest(new ApiResponse<ReportResponse> { Success = false, Message = "Create report failed.", Errors = errors.ToList() });

            return Ok(new ApiResponse<ReportResponse> { Success = true, Message = "Report created.", Data = data });
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin,Teacher")]
        [ProducesResponseType(typeof(ApiResponse<ReportResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Update(int id, [FromBody] UpdateReportRequest request)
        {
            if (id <= 0)
                return BadRequest(new ApiResponse<ReportResponse> { Success = false, Message = "Invalid report id." });

            var (ok, notFound, errors, data) = await _reportService.UpdateReportAsync(id, request);

            if (notFound)
                return NotFound(new ApiResponse<ReportResponse> { Success = false, Message = "Report not found." });

            if (!ok)
                return BadRequest(new ApiResponse<ReportResponse> { Success = false, Message = "Update report failed.", Errors = errors.ToList() });

            return Ok(new ApiResponse<ReportResponse> { Success = true, Message = "Report updated.", Data = data });
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Delete(int id)
        {
            if (id <= 0)
                return BadRequest(new ApiResponse<bool> { Success = false, Message = "Invalid report id." });

            var (ok, notFound, errors) = await _reportService.DeleteReportAsync(id);

            if (notFound)
                return NotFound(new ApiResponse<bool> { Success = false, Message = "Report not found.", Data = false });

            if (!ok)
                return BadRequest(new ApiResponse<bool> { Success = false, Message = "Delete report failed.", Errors = errors.ToList(), Data = false });

            return Ok(new ApiResponse<bool> { Success = true, Message = "Report deleted.", Data = true });
        }
    }
}