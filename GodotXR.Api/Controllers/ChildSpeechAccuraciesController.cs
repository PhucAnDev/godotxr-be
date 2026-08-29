using GodotXR.Application.DTOs.Response;
using GodotXR.Application.DTOs.Request.ChildSpeechAccuracy;
using GodotXR.Application.DTOs.Response.ChildSpeechAccuracy;
using GodotXR.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace GodotXR.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChildSpeechAccuraciesController : ControllerBase
    {
        private readonly IChildSpeechAccuracyService _service;

        public ChildSpeechAccuraciesController(IChildSpeechAccuracyService service)
        {
            _service = service;
        }

        [HttpGet("child/{childId:int}")]
        public async Task<IActionResult> GetByChildId(int childId)
        {
            var data = await _service.GetByChildIdAsync(childId);
            return Ok(new ApiResponse<IEnumerable<ChildSpeechAccuracyResponse>>
            {
                Success = true,
                Message = "OK",
                Data = data
            });
        }

        [HttpGet("session/{sessionId}")]
        public async Task<IActionResult> GetBySessionId(string sessionId)
        {
            var data = await _service.GetBySessionIdAsync(sessionId);
            return Ok(new ApiResponse<IEnumerable<ChildSpeechAccuracyResponse>>
            {
                Success = true,
                Message = "OK",
                Data = data
            });
        }

        [HttpGet("lesson/{lessonId:int}")]
        public async Task<IActionResult> GetByLessonId(int lessonId)
        {
            var data = await _service.GetByLessonIdAsync(lessonId);
            return Ok(new ApiResponse<IEnumerable<ChildSpeechAccuracyResponse>>
            {
                Success = true,
                Message = "OK",
                Data = data
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateChildSpeechAccuracyRequest request)
        {
            var data = await _service.CreateAsync(request);
            return Ok(new ApiResponse<ChildSpeechAccuracyResponse>
            {
                Success = true,
                Message = "Created",
                Data = data
            });
        }

        [HttpPost("batch")]
        public async Task<IActionResult> CreateBatch([FromBody] IEnumerable<CreateChildSpeechAccuracyRequest> requests)
        {
            var count = await _service.CreateBatchAsync(requests);
            return Ok(new ApiResponse<int>
            {
                Success = true,
                Message = $"Inserted {count} speech accuracy records.",
                Data = count
            });
        }
    }
}
