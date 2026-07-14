using GodotXR.Api.Contracts;
using GodotXR.Application.DTOs.Request.Program;
using GodotXR.Application.DTOs.Response;
using GodotXR.Application.DTOs.Response.Program;
using GodotXR.Application.DTOs.Response.User;
using GodotXR.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GodotXR.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProgramsController : ControllerBase
    {
        private readonly IProgramService _programService;

        public ProgramsController(IProgramService programService)
        {
            _programService = programService;
        }


        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PagedResponse<ProgramResponse>>), StatusCodes.Status200OK)]
        public async Task<ActionResult> Get([FromQuery] PaginationQuery query)
        {
            var data = await _programService.GetListProgramAsync(
                query.PageNumber,
                query.PageSize);

            return Ok(new ApiResponse<PagedResponse<ProgramResponse>>
            {
                Success = true,
                Message = "OK",
                Data = data,
            });
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<ProgramResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<ProgramResponse>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<ProgramResponse>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new ApiResponse<ProgramResponse>
                {
                    Success = false,
                    Message = "Invalid program id."
                });
            }

            var data = await _programService.GetProgramByIdAsync(id);

            if (data == null)
            {
                return NotFound(new ApiResponse<ProgramResponse>
                {
                    Success = false,
                    Message = "Program not found."
                });
            }

            return Ok(new ApiResponse<ProgramResponse>
            {
                Success = true,
                Message = "OK",
                Data = data
            });
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<ProgramResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<ProgramResponse>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(
            [FromBody] CreateProgramRequest request)
        {
            var (ok, errors, data)
                = await _programService.CreateProgramAsync(request);

            if (!ok || data == null)
            {
                return BadRequest(new ApiResponse<ProgramResponse>
                {
                    Success = false,
                    Message = "Create program failed.",
                    Errors = errors.ToList()
                });
            }

            return Ok(new ApiResponse<ProgramResponse>
            {
                Success = true,
                Message = "Program created.",
                Data = data
            });
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<ProgramResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<ProgramResponse>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<ProgramResponse>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] UpdateProgramRequest request)
        {
            if (id <= 0)
            {
                return BadRequest(new ApiResponse<UserResponse>
                {
                    Success = false,
                    Message = "Invalid program id."
                });
            }

            var (ok, notFound, errors, data)
                = await _programService.UpdateProgramAsync(id, request);

            if (notFound)
            {
                return NotFound(new ApiResponse<ProgramResponse>
                {
                    Success = false,
                    Message = "Program not found."
                });
            }

            if (!ok || data == null)
            {
                return BadRequest(new ApiResponse<ProgramResponse>
                {
                    Success = false,
                    Message = "Update program failed.",
                    Errors = errors.ToList()
                });
            }

            return Ok(new ApiResponse<ProgramResponse>
            {
                Success = true,
                Message = "Program updated.",
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
                return BadRequest(new ApiResponse<UserResponse>
                {
                    Success = false,
                    Message = "Invalid program id."
                });
            }

            var (ok, notFound, errors)
                = await _programService.DeleteProgramAsync(id);

            if (notFound)
            {
                return NotFound(new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Program not found.",
                    Data = false
                });
            }

            if (!ok)
            {
                return BadRequest(new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Delete program failed.",
                    Errors = errors.ToList(),
                    Data = false
                });
            }

            return Ok(new ApiResponse<bool>
            {
                Success = true,
                Message = "Program deleted.",
                Data = true
            });
        }
    }
}