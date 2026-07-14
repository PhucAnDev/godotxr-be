using AutoMapper;
using GodotXR.Api.Extensions;
using GodotXR.Application.DTOs.Response;
using GodotXR.Application.DTOs.Response.EventLog;
using GodotXR.Domain.IUnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GodotXR.Api.Controllers
{
    [ApiController]
    [Route("api/event-logs")]
    [Authorize]
    public class EventLogsController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public EventLogsController(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        [HttpGet("by-result/{resultId}")]
        [Authorize(Roles = "Admin,Teacher,Parent")]
        public async Task<IActionResult> GetByResult(int resultId)
        {
            var currentUserId = User.GetUserId();

            var result = await _uow.ResultRepository.GetByIdAsync(resultId);
            
            if (result == null)
                return NotFound(new ApiResponse { Success = false, Message = "Result not found." });

            if (User.IsInRole("Parent"))
            {
                var child = await _uow.ChildProfileRepository.GetByIdAsync(result.ChildId);
                if (child == null || child.UserId != currentUserId)
                    return Forbid();
            }
            
            if (User.IsInRole("Teacher"))
            {
                var hasAccess = await _uow.EnrollmentRepository
                    .HasTeacherAccessToChildAsync(currentUserId, result.ChildId);
                if (!hasAccess)
                    return Forbid();
            }

            var logs = await _uow.EventLogRepository.GetByResultIdAsync(resultId);
            
            return Ok(new ApiResponse<IEnumerable<EventLogResponse>>
            {
                Success = true,
                Message = "OK",
                Data = _mapper.Map<IEnumerable<EventLogResponse>>(logs)
            });
        }

        [HttpGet("by-child/{childId}")]
        [Authorize(Roles = "Admin,Teacher,Parent")]
        public async Task<IActionResult> GetByChild(int childId)
        {
            var currentUserId = User.GetUserId();

            var child = await _uow.ChildProfileRepository.GetByIdAsync(childId);
            
            if (child == null)
                return NotFound(new ApiResponse { Success = false, Message = "Child not found." });

            if (User.IsInRole("Parent") && child.UserId != currentUserId)
                return Forbid();

            if (User.IsInRole("Teacher"))
            {
                var hasAccess = await _uow.EnrollmentRepository
                    .HasTeacherAccessToChildAsync(currentUserId, childId);
                if (!hasAccess)
                    return Forbid();
            }

            var logs = await _uow.EventLogRepository.GetByChildIdAsync(childId);
            
            return Ok(new ApiResponse<IEnumerable<EventLogResponse>>
            {
                Success = true,
                Message = "OK",
                Data = _mapper.Map<IEnumerable<EventLogResponse>>(logs)
            });
        }

    }
}