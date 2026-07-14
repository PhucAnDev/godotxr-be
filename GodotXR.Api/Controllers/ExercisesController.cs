using GodotXR.Api.Contracts;
using GodotXR.Application.DTOs.Request.Exercise;
using GodotXR.Application.DTOs.Response;
using GodotXR.Application.DTOs.Response.Exercise;
using GodotXR.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace GodotXR.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ExercisesController : ControllerBase
    {
        private readonly IExerciseService _service;
        public ExercisesController(IExerciseService service) => _service = service;

        [HttpGet]
        [Authorize(Roles = "Admin,Teacher,Parent")]
        public async Task<IActionResult> Get(
            [FromQuery] PaginationQuery query,
            [FromQuery] int? lessonId = null,
            [FromQuery] int? typeId = null,
            [FromQuery] int? teacherId = null,
            [FromQuery] string? status = null)
        {
            var data = await _service.GetListAsync(query.PageNumber, query.PageSize, lessonId, typeId, teacherId, status);
            return Ok(new ApiResponse<PagedResponse<ExerciseResponse>> { Success = true, Message = "OK", Data = data });
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = "Admin,Teacher,Parent")]
        public async Task<IActionResult> GetById(int id)
        {
            if (id <= 0) return BadRequest(new ApiResponse<ExerciseResponse> { Success = false, Message = "Invalid id." });
            var data = await _service.GetByIdAsync(id);
            if (data == null) return NotFound(new ApiResponse<ExerciseResponse> { Success = false, Message = "Exercise not found." });
            return Ok(new ApiResponse<ExerciseResponse> { Success = true, Message = "OK", Data = data });
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> Create([FromBody] CreateExerciseRequest request)
        {
            var (ok, errors, data) = await _service.CreateAsync(request);
            if (!ok || data == null)
                return BadRequest(new ApiResponse<ExerciseResponse> { Success = false, Message = "Create failed.", Errors = errors.ToList() });
            return Ok(new ApiResponse<ExerciseResponse> { Success = true, Message = "Exercise created.", Data = data });
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateExerciseRequest request)
        {
            if (id <= 0) return BadRequest(new ApiResponse<ExerciseResponse> { Success = false, Message = "Invalid id." });
            var (ok, notFound, errors, data) = await _service.UpdateAsync(id, request);
            if (notFound) return NotFound(new ApiResponse<ExerciseResponse> { Success = false, Message = "Exercise not found." });
            if (!ok || data == null)
                return BadRequest(new ApiResponse<ExerciseResponse> { Success = false, Message = "Update failed.", Errors = errors.ToList() });
            return Ok(new ApiResponse<ExerciseResponse> { Success = true, Message = "Exercise updated.", Data = data });
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest(new ApiResponse<bool> { Success = false, Message = "Invalid id." });
            var (ok, notFound, errors) = await _service.DeleteAsync(id);
            if (notFound) return NotFound(new ApiResponse<bool> { Success = false, Message = "Exercise not found.", Data = false });
            if (!ok) return BadRequest(new ApiResponse<bool> { Success = false, Message = "Delete failed.", Errors = errors.ToList(), Data = false });
            return Ok(new ApiResponse<bool> { Success = true, Message = "Exercise deleted.", Data = true });
        }
    }
}