using GodotXR.Api.Contracts;
using GodotXR.Application.DTOs.Request.Semester;
using GodotXR.Application.DTOs.Response;
using GodotXR.Application.DTOs.Response.Semester;
using GodotXR.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GodotXR.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class SemestersController : ControllerBase
    {
        private readonly ISemesterService _semesterService;

        public SemestersController(ISemesterService semesterService)
        {
            _semesterService = semesterService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PagedResponse<SemesterResponse>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get(
            [FromQuery] PaginationQuery query,
            [FromQuery] int? schoolYearId = null,
            [FromQuery] string? status = null)
        {
            var data = await _semesterService.GetListSemesterAsync(
                query.PageNumber, query.PageSize, schoolYearId, status);

            return Ok(new ApiResponse<PagedResponse<SemesterResponse>>
            {
                Success = true,
                Message = "OK",
                Data = data
            });
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<SemesterResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            if (id <= 0)
                return BadRequest(new ApiResponse<SemesterResponse>
                {
                    Success = false,
                    Message = "Invalid semester id."
                });

            var data = await _semesterService.GetSemesterByIdAsync(id);

            if (data == null)
                return NotFound(new ApiResponse<SemesterResponse>
                {
                    Success = false,
                    Message = "Semester not found."
                });

            return Ok(new ApiResponse<SemesterResponse>
            {
                Success = true,
                Message = "OK",
                Data = data
            });
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<SemesterResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateSemesterRequest request)
        {
            var (ok, errors, data) = await _semesterService.CreateSemesterAsync(request);

            if (!ok || data == null)
                return BadRequest(new ApiResponse<SemesterResponse>
                {
                    Success = false,
                    Message = "Create semester failed.",
                    Errors = errors.ToList()
                });

            return Ok(new ApiResponse<SemesterResponse>
            {
                Success = true,
                Message = "Semester created.",
                Data = data
            });
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<SemesterResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateSemesterRequest request)
        {
            if (id <= 0)
                return BadRequest(new ApiResponse<SemesterResponse>
                {
                    Success = false,
                    Message = "Invalid semester id."
                });

            var (ok, notFound, errors, data) = await _semesterService.UpdateSemesterAsync(id, request);

            if (notFound)
                return NotFound(new ApiResponse<SemesterResponse>
                {
                    Success = false,
                    Message = "Semester not found."
                });

            if (!ok || data == null)
                return BadRequest(new ApiResponse<SemesterResponse>
                {
                    Success = false,
                    Message = "Update semester failed.",
                    Errors = errors.ToList()
                });

            return Ok(new ApiResponse<SemesterResponse>
            {
                Success = true,
                Message = "Semester updated.",
                Data = data
            });
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
                return BadRequest(new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Invalid semester id."
                });

            var (ok, notFound, errors) = await _semesterService.DeleteSemesterAsync(id);

            if (notFound)
                return NotFound(new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Semester not found.",
                    Data = false
                });

            if (!ok)
                return BadRequest(new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Delete semester failed.",
                    Errors = errors.ToList(),
                    Data = false
                });

            return Ok(new ApiResponse<bool>
            {
                Success = true,
                Message = "Semester deleted.",
                Data = true
            });
        }
    }
}