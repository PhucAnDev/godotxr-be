using GodotXR.Api.Contracts;
using GodotXR.Application.DTOs.Request.ExerciseType;
using GodotXR.Application.DTOs.Response;
using GodotXR.Application.DTOs.Response.ExerciseType;
using GodotXR.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace GodotXR.Api.Controllers
{
    [ApiController]
    [Route("api/exercise-types")]
    [Authorize]
    public class ExerciseTypesController : ControllerBase
    {
        private readonly IExerciseTypeService _service;
        public ExerciseTypesController(IExerciseTypeService service) => _service = service;

        [HttpGet]
        [Authorize(Roles = "Admin,Teacher,Parent")]
        public async Task<IActionResult> Get([FromQuery] PaginationQuery query, [FromQuery] bool? isActive = null)
        {
            var data = await _service.GetListAsync(query.PageNumber, query.PageSize, isActive);
            return Ok(new ApiResponse<PagedResponse<ExerciseTypeResponse>> { Success = true, Message = "OK", Data = data });
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = "Admin,Teacher,Parent")] 
        public async Task<IActionResult> GetById(int id)
        {
            if (id <= 0) return BadRequest(new ApiResponse<ExerciseTypeResponse> { Success = false, Message = "Invalid id." });
            var data = await _service.GetByIdAsync(id);
            if (data == null) return NotFound(new ApiResponse<ExerciseTypeResponse> { Success = false, Message = "Exercise type not found." });
            return Ok(new ApiResponse<ExerciseTypeResponse> { Success = true, Message = "OK", Data = data });
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateExerciseTypeRequest request)
        {
            var (ok, errors, data) = await _service.CreateAsync(request);
            if (!ok || data == null)
                return BadRequest(new ApiResponse<ExerciseTypeResponse> { Success = false, Message = "Create failed.", Errors = errors.ToList() });
            return Ok(new ApiResponse<ExerciseTypeResponse> { Success = true, Message = "Exercise type created.", Data = data });
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateExerciseTypeRequest request)
        {
            if (id <= 0) return BadRequest(new ApiResponse<ExerciseTypeResponse> { Success = false, Message = "Invalid id." });
            var (ok, notFound, errors, data) = await _service.UpdateAsync(id, request);
            if (notFound) return NotFound(new ApiResponse<ExerciseTypeResponse> { Success = false, Message = "Exercise type not found." });
            if (!ok || data == null)
                return BadRequest(new ApiResponse<ExerciseTypeResponse> { Success = false, Message = "Update failed.", Errors = errors.ToList() });
            return Ok(new ApiResponse<ExerciseTypeResponse> { Success = true, Message = "Exercise type updated.", Data = data });
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest(new ApiResponse<bool> { Success = false, Message = "Invalid id." });
            var (ok, notFound, errors) = await _service.DeleteAsync(id);
            if (notFound) return NotFound(new ApiResponse<bool> { Success = false, Message = "Exercise type not found.", Data = false });
            if (!ok) return BadRequest(new ApiResponse<bool> { Success = false, Message = "Delete failed.", Errors = errors.ToList(), Data = false });
            return Ok(new ApiResponse<bool> { Success = true, Message = "Exercise type deleted.", Data = true });
        }
    }
}