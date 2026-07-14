using GodotXR.Api.Contracts;
using GodotXR.Application.DTOs.Request.Lesson;
using GodotXR.Application.DTOs.Response;
using GodotXR.Application.DTOs.Response.Lesson;
using GodotXR.Application.DTOs.Response.User;
using GodotXR.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace GodotXR.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LessonsController : ControllerBase
    {
        private readonly ILessonService _lessonService;

        public LessonsController(ILessonService lessonService)
        {
            _lessonService = lessonService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PagedResponse<LessonResponse>>), StatusCodes.Status200OK)]
        public async Task<ActionResult> Get([FromQuery] PaginationQuery query)
        {
            var data = await _lessonService.GetListLessonAsync(
                query.PageNumber,
                query.PageSize);

            return Ok(new ApiResponse<PagedResponse<LessonResponse>>
            {
                Success = true,
                Message = "OK",
                Data = data,
            });
        }

        [HttpGet("program/{programId:int}")]
        public async Task<IActionResult> GetByProgramId(int programId)
        {
            if (programId <= 0)
            {
                return BadRequest(new ApiResponse<IEnumerable<LessonResponse>>
                {
                    Success = false,
                    Message = "Invalid program id."
                });
            }

            var data = await _lessonService.GetLessonByProgramIdAsync(programId);

            return Ok(new ApiResponse<IEnumerable<LessonResponse>>
            {
                Success = true,
                Message = "OK",
                Data = data
            });
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<LessonResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<LessonResponse>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<LessonResponse>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new ApiResponse<LessonResponse>
                {
                    Success = false,
                    Message = "Invalid lesson id."
                });
            }

            var data = await _lessonService.GetLessonByIdAsync(id);

            if (data == null)
            {
                return NotFound(new ApiResponse<LessonResponse>
                {
                    Success = false,
                    Message = "Lesson not found."
                });
            }

            return Ok(new ApiResponse<LessonResponse>
            {
                Success = true,
                Message = "OK",
                Data = data
            });
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<LessonResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<LessonResponse>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(
            [FromBody] CreateLessonRequest request)
        {
            var (ok, errors, data)
                = await _lessonService.CreateLessonAsync(request);

            if (!ok || data == null)
            {
                return BadRequest(new ApiResponse<LessonResponse>
                {
                    Success = false,
                    Message = "Create lesson failed.",
                    Errors = errors.ToList()
                });
            }

            return Ok(new ApiResponse<LessonResponse>
            {
                Success = true,
                Message = "Lesson created.",
                Data = data
            });
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<LessonResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<LessonResponse>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<LessonResponse>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] UpdateLessonRequest request)
        {
            if (id <= 0)
            {
                return BadRequest(new ApiResponse<LessonResponse>
                {
                    Success = false,
                    Message = "Invalid lesson id."
                });
            }

            var (ok, notFound, errors, data)
                = await _lessonService.UpdateLessonAsync(id, request);

            if (notFound)
            {
                return NotFound(new ApiResponse<LessonResponse>
                {
                    Success = false,
                    Message = "Lesson not found."
                });
            }

            if (!ok || data == null)
            {
                return BadRequest(new ApiResponse<LessonResponse>
                {
                    Success = false,
                    Message = "Update lesson failed.",
                    Errors = errors.ToList()
                });
            }

            return Ok(new ApiResponse<LessonResponse>
            {
                Success = true,
                Message = "Lesson updated.",
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
                    Message = "Invalid lesson id."
                });
            }

            var (ok, notFound, errors)
                = await _lessonService.DeleteLessonAsync(id);

            if (notFound)
            {
                return NotFound(new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Lesson not found.",
                    Data = false
                });
            }

            if (!ok)
            {
                return BadRequest(new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Delete lesson failed.",
                    Errors = errors.ToList(),
                    Data = false
                });
            }

            return Ok(new ApiResponse<bool>
            {
                Success = true,
                Message = "Lesson deleted.",
                Data = true
            });
        }
    }
}