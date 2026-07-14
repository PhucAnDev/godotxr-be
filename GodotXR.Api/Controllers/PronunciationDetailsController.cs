using AutoMapper;
using GodotXR.Api.Extensions;
using GodotXR.Application.DTOs.Response;
using GodotXR.Application.DTOs.Response.PronunciationDetail;
using GodotXR.Domain.IUnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GodotXR.Api.Controllers
{
    [ApiController]
    [Route("api/pronunciation-details")]
    [Authorize]
    public class PronunciationDetailsController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public PronunciationDetailsController(IUnitOfWork uow, IMapper mapper)
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

            var details = await _uow.PronunciationDetailRepository.GetByResultIdAsync(resultId);
            
            return Ok(new ApiResponse<IEnumerable<PronunciationDetailResponse>>
            {
                Success = true,
                Message = "OK",
                Data = _mapper.Map<IEnumerable<PronunciationDetailResponse>>(details)
            });
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Teacher,Parent")]
        public async Task<IActionResult> GetById(int id)
        {
            var currentUserId = User.GetUserId();

            var detail = await _uow.PronunciationDetailRepository.GetByIdAsync(id);
            
            if (detail == null)
                return NotFound(new ApiResponse { Success = false, Message = "Pronunciation detail not found." });
            
            var result = await _uow.ResultRepository.GetByIdAsync(detail.ResultId);
            
            if (result != null)
            {
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
            }

            return Ok(new ApiResponse<PronunciationDetailResponse>
            {
                Success = true,
                Message = "OK",
                Data = _mapper.Map<PronunciationDetailResponse>(detail)
            });
        }

    }
}