using GodotXR.Application.DTOs.Request.LessonImage;
using GodotXR.Application.DTOs.Request.LessonSlot;
using GodotXR.Application.DTOs.Response;
using GodotXR.Application.DTOs.Response.LessonImage;
using GodotXR.Application.DTOs.Response.LessonSlot;
using GodotXR.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GodotXR.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Tags("LessonExercises")]
    public class LessonExercisesController : ControllerBase
    {
        private readonly ILessonSlotService _lessonSlotService;

        public LessonExercisesController(ILessonSlotService lessonSlotService)
        {
            _lessonSlotService = lessonSlotService;
        }

        #region Lesson Scenes (Bối cảnh / Góc nhìn không gian bài học)

        [HttpGet("api/lessons/{lessonId:int}/scenes")]
        [Authorize(Roles = "Admin,Teacher,Parent")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<LessonImageResponse>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetScenes(int lessonId)
        {
            var images = await _lessonSlotService.GetImagesByLessonIdAsync(lessonId);
            return Ok(new ApiResponse<IEnumerable<LessonImageResponse>>
            {
                Success = true,
                Message = "Get lesson scenes successfully.",
                Data = images
            });
        }

        [HttpPost("api/lessons/{lessonId:int}/scenes")]
        [Authorize(Roles = "Admin,Teacher,Parent")]
        [ProducesResponseType(typeof(ApiResponse<LessonImageResponse>), StatusCodes.Status201Created)]
        public async Task<IActionResult> UploadScene(int lessonId, [FromForm] UploadLessonImageRequest request)
        {
            using var stream = request.ImageFile.OpenReadStream();
            try
            {
                var result = await _lessonSlotService.AddImageAsync(
                    lessonId,
                    request.AngleName,
                    stream,
                    request.ImageFile.FileName,
                    request.ImageFile.ContentType
                );

                return Created("", new ApiResponse<LessonImageResponse>
                {
                    Success = true,
                    Message = "Upload lesson scene successfully.",
                    Data = result
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse<LessonImageResponse>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        [HttpDelete("api/lessons/{lessonId:int}/scenes/{sceneId:int}")]
        [Authorize(Roles = "Admin,Teacher,Parent")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteScene(int lessonId, int sceneId)
        {
            var succeeded = await _lessonSlotService.DeleteImageAsync(lessonId, sceneId);
            if (!succeeded)
            {
                return NotFound(new ApiResponse
                {
                    Success = false,
                    Message = "Lesson scene not found."
                });
            }

            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Lesson scene deleted successfully."
            });
        }

        #endregion

        #region Lesson Exercises (Bài tập / Nhiệm vụ luyện tập tương tác)

        [HttpGet("api/lessons/{lessonId:int}/exercises")]
        [Authorize(Roles = "Admin,Teacher,Parent")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<LessonSlotResponse>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetExercises(int lessonId)
        {
            var slots = await _lessonSlotService.GetSlotsByLessonIdAsync(lessonId);
            return Ok(new ApiResponse<IEnumerable<LessonSlotResponse>>
            {
                Success = true,
                Message = "Get lesson exercises successfully.",
                Data = slots
            });
        }

        [HttpPost("api/lessons/{lessonId:int}/exercises")]
        [Authorize(Roles = "Admin,Teacher,Parent")]
        [ProducesResponseType(typeof(ApiResponse<LessonSlotResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ConfigureExercise(int lessonId, [FromBody] ConfigureSlotRequest request)
        {
            try
            {
                var result = await _lessonSlotService.ConfigureSlotAsync(
                    lessonId,
                    request.SlotName,
                    request.LessonImageId,
                    request.CorrectPoints,
                    request.WrongPoints
                );

                return Ok(new ApiResponse<LessonSlotResponse>
                {
                    Success = true,
                    Message = "Configure exercise successfully.",
                    Data = result
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse<LessonSlotResponse>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        [HttpPut("api/lessons/{lessonId:int}/exercises/{id:int}")]
        [Authorize(Roles = "Admin,Teacher,Parent")]
        [ProducesResponseType(typeof(ApiResponse<LessonSlotResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateExercise(int lessonId, int id, [FromBody] ConfigureSlotRequest request)
        {
            try
            {
                var result = await _lessonSlotService.UpdateSlotAsync(
                    lessonId,
                    id,
                    request.SlotName,
                    request.LessonImageId,
                    request.CorrectPoints,
                    request.WrongPoints
                );

                if (result == null)
                {
                    return NotFound(new ApiResponse<LessonSlotResponse>
                    {
                        Success = false,
                        Message = "Bài tập không tồn tại trong bài học này."
                    });
                }

                return Ok(new ApiResponse<LessonSlotResponse>
                {
                    Success = true,
                    Message = "Cập nhật bài tập thành công.",
                    Data = result
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse<LessonSlotResponse>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        [HttpDelete("api/lessons/{lessonId:int}/exercises/{id:int}")]
        [Authorize(Roles = "Admin,Teacher,Parent")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteExercise(int lessonId, int id)
        {
            var succeeded = await _lessonSlotService.DeleteSlotAsync(lessonId, id);
            if (!succeeded)
            {
                return NotFound(new ApiResponse
                {
                    Success = false,
                    Message = "Bài tập không tồn tại hoặc đã bị xóa."
                });
            }

            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Xóa bài tập thành công."
            });
        }

        [HttpPut("api/lessons/{lessonId:int}/exercises/{id:int}/assign-asset")]
        [Authorize(Roles = "Admin,Teacher,Parent")]
        [ProducesResponseType(typeof(ApiResponse<LessonSlotResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> AssignAssetToExercise(int lessonId, int id, [FromBody] AssignItemAssetRequest request)
        {
            try
            {
                var result = await _lessonSlotService.AssignItemToSlotAsync(lessonId, id, request.ItemAssetId);
                if (result == null)
                {
                    return NotFound(new ApiResponse<LessonSlotResponse>
                    {
                        Success = false,
                        Message = "Exercise not found in this lesson."
                    });
                }

                return Ok(new ApiResponse<LessonSlotResponse>
                {
                    Success = true,
                    Message = "Asset assigned to exercise successfully.",
                    Data = result
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse<LessonSlotResponse>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        [HttpGet("api/lessons/{lessonId:int}/client-config")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ClientConfigSlotResponse>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetClientConfig(int lessonId)
        {
            var config = await _lessonSlotService.GetClientConfigAsync(lessonId);
            
            var clientConfig = config.Select(slot => new ClientConfigSlotResponse
            {
                Id = slot.Id,
                LessonId = slot.LessonId,
                SlotName = slot.SlotName,
                CorrectPoints = slot.CorrectPoints,
                WrongPoints = slot.WrongPoints,
                ItemAsset = slot.ItemAsset != null ? new ClientConfigAssetResponse
                {
                    Id = slot.ItemAsset.Id,
                    ItemName = slot.ItemAsset.Name
                } : null
            });

            return Ok(new ApiResponse<IEnumerable<ClientConfigSlotResponse>>
            {
                Success = true,
                Message = "Get VR client config successfully.",
                Data = clientConfig
            });
        }

        #endregion
    }
}
