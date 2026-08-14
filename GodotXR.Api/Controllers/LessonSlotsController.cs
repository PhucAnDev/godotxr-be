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
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading.Tasks;

namespace GodotXR.Api.Controllers
{

    [ApiController]
    [Authorize]
    public class LessonSlotsController : ControllerBase
    {
        private readonly ILessonSlotService _lessonSlotService;

        public LessonSlotsController(ILessonSlotService lessonSlotService)
        {
            _lessonSlotService = lessonSlotService;
        }

        #region LessonImages

        [HttpGet("api/lesson-images/{lessonId:int}")]
        [Authorize(Roles = "Admin,Teacher")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<LessonImageResponse>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetImages(int lessonId)
        {
            var images = await _lessonSlotService.GetImagesByLessonIdAsync(lessonId);
            return Ok(new ApiResponse<IEnumerable<LessonImageResponse>>
            {
                Success = true,
                Message = "Get lesson images successfully.",
                Data = images
            });
        }

        [HttpPost("api/lesson-images/{lessonId:int}")]
        [Authorize(Roles = "Admin,Teacher")]
        [ProducesResponseType(typeof(ApiResponse<LessonImageResponse>), StatusCodes.Status201Created)]
        public async Task<IActionResult> UploadImage(int lessonId, [FromForm] UploadLessonImageRequest request)
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
                    Message = "Upload lesson image successfully.",
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

        [HttpDelete("api/lesson-images/{lessonId:int}/{imageId:int}")]
        [Authorize(Roles = "Admin,Teacher")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteImage(int lessonId, int imageId)
        {
            var succeeded = await _lessonSlotService.DeleteImageAsync(lessonId, imageId);
            if (!succeeded)
            {
                return NotFound(new ApiResponse
                {
                    Success = false,
                    Message = "Lesson image not found."
                });
            }

            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Lesson image deleted successfully."
            });
        }

        #endregion

        #region LessonSlots

        [HttpGet("api/lesson-slots/{lessonId:int}")]
        [Authorize(Roles = "Admin,Teacher")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<LessonSlotResponse>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSlots(int lessonId)
        {
            var slots = await _lessonSlotService.GetSlotsByLessonIdAsync(lessonId);
            return Ok(new ApiResponse<IEnumerable<LessonSlotResponse>>
            {
                Success = true,
                Message = "Get lesson slots successfully.",
                Data = slots
            });
        }

        [HttpPost("api/lesson-slots/{lessonId:int}")]
        [Authorize(Roles = "Admin,Teacher")]
        [ProducesResponseType(typeof(ApiResponse<LessonSlotResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ConfigureSlot(int lessonId, [FromBody] ConfigureSlotRequest request)
        {
            try
            {
                string slotIdentifier = GenerateSlotIdentifier(request.SlotName);
                var result = await _lessonSlotService.ConfigureSlotAsync(
                    lessonId,
                    slotIdentifier,
                    request.SlotName,
                    request.LessonImageId
                );

                return Ok(new ApiResponse<LessonSlotResponse>
                {
                    Success = true,
                    Message = "Configure slot successfully.",
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

        private static string GenerateSlotIdentifier(string slotName)
        {
            if (string.IsNullOrWhiteSpace(slotName))
                return "slot_" + Guid.NewGuid().ToString("N").Substring(0, 8);

            string str = slotName.ToLowerInvariant();
            var normalizedString = str.Normalize(System.Text.NormalizationForm.FormD);
            var stringBuilder = new System.Text.StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != System.Globalization.UnicodeCategory.NonSpacingMark)
                {
                    if (c == 'đ')
                        stringBuilder.Append('d');
                    else
                        stringBuilder.Append(c);
                }
            }

            string clean = stringBuilder.ToString().Normalize(System.Text.NormalizationForm.FormC);
            var builder = new System.Text.StringBuilder();
            bool prevWasUnderscore = false;
            foreach (char c in clean)
            {
                if (char.IsLetterOrDigit(c))
                {
                    builder.Append(c);
                    prevWasUnderscore = false;
                }
                else
                {
                    if (!prevWasUnderscore)
                    {
                        builder.Append('_');
                        prevWasUnderscore = true;
                    }
                }
            }

            string result = builder.ToString().Trim('_');
            string suffix = Guid.NewGuid().ToString("N").Substring(0, 6);
            result = $"{result}_{suffix}";

            if (result.Length > 150)
                result = result.Substring(0, 150);

            return result;
        }

        [HttpPut("api/lesson-slots/{lessonId:int}/{id:int}/assign")]
        [Authorize(Roles = "Admin,Teacher")]
        [ProducesResponseType(typeof(ApiResponse<LessonSlotResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> AssignItemToSlot(int lessonId, int id, [FromBody] AssignItemAssetRequest request)
        {
            try
            {
                var result = await _lessonSlotService.AssignItemToSlotAsync(lessonId, id, request.ItemAssetId);
                if (result == null)
                {
                    return NotFound(new ApiResponse<LessonSlotResponse>
                    {
                        Success = false,
                        Message = "Slot not found in this lesson."
                    });
                }

                return Ok(new ApiResponse<LessonSlotResponse>
                {
                    Success = true,
                    Message = "Item assigned to slot successfully.",
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
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<LessonSlotResponse>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetClientConfig(int lessonId)
        {
            var config = await _lessonSlotService.GetClientConfigAsync(lessonId);
            return Ok(new ApiResponse<IEnumerable<LessonSlotResponse>>
            {
                Success = true,
                Message = "Get VR client config successfully.",
                Data = config
            });
        }

        #endregion
    }
}
