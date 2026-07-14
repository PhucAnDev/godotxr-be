using GodotXR.Api.Contracts;
using GodotXR.Application.DTOs.Request.SchoolYear;
using GodotXR.Application.DTOs.Response;
using GodotXR.Application.DTOs.Response.SchoolYear;
using GodotXR.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GodotXR.Api.Controllers
{
    [ApiController]
    [Route("api/school-years")]
    [Authorize(Roles = "Admin")]   
    public class SchoolYearsController : ControllerBase
    {
        private readonly ISchoolYearService _schoolYearService;

        public SchoolYearsController(ISchoolYearService schoolYearService)
        {
            _schoolYearService = schoolYearService;
        }
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PagedResponse<SchoolYearResponse>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get(
            [FromQuery] PaginationQuery query,
            [FromQuery] string? status = null,
            [FromQuery] string? search = null)
        {
            var data = await _schoolYearService.GetListSchoolYearAsync(
                query.PageNumber,
                query.PageSize,
                status,
                search);

            return Ok(new ApiResponse<PagedResponse<SchoolYearResponse>>
            {
                Success = true,
                Message = "OK",
                Data = data
            });
        }
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<SchoolYearResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            if (id <= 0)
                return BadRequest(new ApiResponse<SchoolYearResponse>
                {
                    Success = false,
                    Message = "Invalid school year id."
                });

            var data = await _schoolYearService.GetSchoolYearByIdAsync(id);

            if (data == null)
                return NotFound(new ApiResponse<SchoolYearResponse>
                {
                    Success = false,
                    Message = "School year not found."
                });

            return Ok(new ApiResponse<SchoolYearResponse>
            {
                Success = true,
                Message = "OK",
                Data = data
            });
        }
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<SchoolYearResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateSchoolYearRequest request)
        {
            var (ok, errors, data) = await _schoolYearService.CreateSchoolYearAsync(request);

            if (!ok || data == null)
                return BadRequest(new ApiResponse<SchoolYearResponse>
                {
                    Success = false,
                    Message = "Create school year failed.",
                    Errors = errors.ToList()
                });

            return Ok(new ApiResponse<SchoolYearResponse>
            {
                Success = true,
                Message = "School year created.",
                Data = data
            });
        }
        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<SchoolYearResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateSchoolYearRequest request)
        {
            if (id <= 0)
                return BadRequest(new ApiResponse<SchoolYearResponse>
                {
                    Success = false,
                    Message = "Invalid school year id."
                });

            var (ok, notFound, errors, data) = await _schoolYearService.UpdateSchoolYearAsync(id, request);

            if (notFound)
                return NotFound(new ApiResponse<SchoolYearResponse>
                {
                    Success = false,
                    Message = "School year not found."
                });

            if (!ok || data == null)
                return BadRequest(new ApiResponse<SchoolYearResponse>
                {
                    Success = false,
                    Message = "Update school year failed.",
                    Errors = errors.ToList()
                });

            return Ok(new ApiResponse<SchoolYearResponse>
            {
                Success = true,
                Message = "School year updated.",
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
                    Message = "Invalid school year id."
                });

            var (ok, notFound, errors) = await _schoolYearService.DeleteSchoolYearAsync(id);

            if (notFound)
                return NotFound(new ApiResponse<bool>
                {
                    Success = false,
                    Message = "School year not found.",
                    Data = false
                });

            if (!ok)
                return BadRequest(new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Delete school year failed.",
                    Errors = errors.ToList(),
                    Data = false
                });

            return Ok(new ApiResponse<bool>
            {
                Success = true,
                Message = "School year deleted.",
                Data = true
            });
        }
        [HttpPatch("{id:int}/set-active")]
        [ProducesResponseType(typeof(ApiResponse<SchoolYearResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SetActive(int id)
        {
            if (id <= 0)
                return BadRequest(new ApiResponse<SchoolYearResponse>
                {
                    Success = false,
                    Message = "Invalid school year id."
                });

            var (ok, notFound, errors, data) = await _schoolYearService.SetActiveSchoolYearAsync(id);

            if (notFound)
                return NotFound(new ApiResponse<SchoolYearResponse>
                {
                    Success = false,
                    Message = "School year not found."
                });

            if (!ok || data == null)
                return BadRequest(new ApiResponse<SchoolYearResponse>
                {
                    Success = false,
                    Message = "Set active failed.",
                    Errors = errors.ToList()
                });

            return Ok(new ApiResponse<SchoolYearResponse>
            {
                Success = true,
                Message = "School year is now active.",
                Data = data
            });
        }
    }
}