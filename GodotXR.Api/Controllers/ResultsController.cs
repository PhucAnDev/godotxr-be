using GodotXR.Api.Extensions;
using GodotXR.Application.DTOs.Request.Result;
using GodotXR.Application.DTOs.Response;
using GodotXR.Application.DTOs.Response.Result;
using GodotXR.Application.Services;
using GodotXR.Domain.IUnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GodotXR.Api.Controllers
{
    [ApiController]
    [Route("api/results")]
    [Authorize]
    public class ResultsController : ControllerBase
    {
        private readonly IResultService _resultService;
        private readonly IUnitOfWork _unitOfWork;

        public ResultsController(IResultService resultService, IUnitOfWork unitOfWork)
        {
            _resultService = resultService;
            _unitOfWork = unitOfWork;
        }

        [HttpPost("submit")]
        [Authorize(Roles = "Parent,Child")]
        public async Task<IActionResult> Submit([FromBody] SubmitResultRequest request)
        {
            var (succeeded, notFound, errors, data) = await _resultService.SubmitAsync(request);

            if (notFound)
                return NotFound(new ApiResponse<ResultResponse>
                {
                    Success = false,
                    Message = "Not found.",
                    Errors = errors.ToList()
                });
            if (!succeeded)
                return BadRequest(new ApiResponse<ResultResponse>
                {
                    Success = false,
                    Message = "Submit failed.",
                    Errors = errors.ToList()
                });

            return Ok(new ApiResponse<ResultResponse>
            {
                Success = true,
                Message = "Result submitted successfully.",
                Data = data
            });
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Teacher,Parent")]
        public async Task<IActionResult> GetById(int id)
        {
            var currentUserId = User.GetUserId();

            var resultResponse = await _resultService.GetByIdAsync(id);
            if (!resultResponse.Success)
                return NotFound(resultResponse);

            var data = resultResponse.Data!;

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

            return Ok(resultResponse);
        }

        [HttpGet("by-child/{childId}")]
        [Authorize(Roles = "Admin,Teacher,Parent")]
        public async Task<IActionResult> GetByChild(int childId)
        {
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

            var result = await _resultService.GetByChildIdAsync(childId);
            if (!result.Success)
                return NotFound(result);
            return Ok(result);
        }

        [HttpGet("by-exercise/{exerciseId}")]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> GetByExercise(int exerciseId)
        {
            var currentUserId = User.GetUserId();

            if (User.IsInRole("Teacher"))
            {
                var exercise = await _unitOfWork.ExerciseRepository.GetByIdAsync(exerciseId);
                if (exercise == null || exercise.TeacherId != currentUserId)
                    return Forbid();
            }

            var result = await _resultService.GetByExerciseIdAsync(exerciseId);
            if (!result.Success)
                return NotFound(result);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            return BadRequest(new ApiResponse
            {
                Success = false,
                Message = "Finalized results cannot be permanently deleted." 
            });
        }
    }
}