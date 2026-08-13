using GodotXR.Api.Contracts;
using GodotXR.Application.DTOs.Request.ItemAsset;
using GodotXR.Application.DTOs.Response;
using GodotXR.Application.DTOs.Response.ItemAsset;
using GodotXR.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace GodotXR.Api.Controllers
{
    [ApiController]
    [Route("api/item-assets")]
    [Authorize]
    [RequestSizeLimit(100 * 1024 * 1024)] // 100 MB request size limit
    public class ItemAssetsController : ControllerBase
    {
        private readonly IItemAssetService _itemAssetService;

        public ItemAssetsController(IItemAssetService itemAssetService)
        {
            _itemAssetService = itemAssetService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Teacher")]
        [ProducesResponseType(typeof(ApiResponse<PagedResponse<ItemAssetResponse>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetList([FromQuery] PaginationQuery query)
        {
            var paged = await _itemAssetService.GetListAsync(query.PageNumber, query.PageSize);
            return Ok(new ApiResponse<PagedResponse<ItemAssetResponse>>
            {
                Success = true,
                Message = "Get item assets successfully.",
                Data = paged
            });
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = "Admin,Teacher")]
        [ProducesResponseType(typeof(ApiResponse<ItemAssetResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _itemAssetService.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound(new ApiResponse<ItemAssetResponse>
                {
                    Success = false,
                    Message = "Item asset not found."
                });
            }

            return Ok(new ApiResponse<ItemAssetResponse>
            {
                Success = true,
                Message = "Get item asset successfully.",
                Data = item
            });
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Teacher")]
        [ProducesResponseType(typeof(ApiResponse<ItemAssetResponse>), StatusCodes.Status201Created)]
        public async Task<IActionResult> Create([FromForm] CreateItemAssetRequest request)
        {
            using var modelStream = request.ModelFile.OpenReadStream();
            
            Stream? imageStream = null;
            if (request.ImageFile != null)
            {
                imageStream = request.ImageFile.OpenReadStream();
            }

            Stream? audioStream = null;
            if (request.AudioFile != null)
            {
                audioStream = request.AudioFile.OpenReadStream();
            }

            try
            {
                var result = await _itemAssetService.CreateAsync(
                    request.Name,
                    request.AnswerSentence,
                    modelStream,
                    request.ModelFile.FileName,
                    request.ModelFile.ContentType,
                    imageStream,
                    request.ImageFile?.FileName,
                    request.ImageFile?.ContentType,
                    audioStream,
                    request.AudioFile?.FileName,
                    request.AudioFile?.ContentType
                );

                return CreatedAtAction(nameof(GetById), new { id = result.Id }, new ApiResponse<ItemAssetResponse>
                {
                    Success = true,
                    Message = "Item asset created successfully.",
                    Data = result
                });
            }
            finally
            {
                imageStream?.Dispose();
                audioStream?.Dispose();
            }
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin,Teacher")]
        [ProducesResponseType(typeof(ApiResponse<ItemAssetResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Update(int id, [FromForm] UpdateItemAssetRequest request)
        {
            Stream? modelStream = null;
            if (request.ModelFile != null)
            {
                modelStream = request.ModelFile.OpenReadStream();
            }

            Stream? imageStream = null;
            if (request.ImageFile != null)
            {
                imageStream = request.ImageFile.OpenReadStream();
            }

            Stream? audioStream = null;
            if (request.AudioFile != null)
            {
                audioStream = request.AudioFile.OpenReadStream();
            }

            try
            {
                var result = await _itemAssetService.UpdateAsync(
                    id,
                    request.Name,
                    request.AnswerSentence,
                    modelStream,
                    request.ModelFile?.FileName,
                    request.ModelFile?.ContentType,
                    imageStream,
                    request.ImageFile?.FileName,
                    request.ImageFile?.ContentType,
                    audioStream,
                    request.AudioFile?.FileName,
                    request.AudioFile?.ContentType
                );

                if (result == null)
                {
                    return NotFound(new ApiResponse<ItemAssetResponse>
                    {
                        Success = false,
                        Message = "Item asset not found."
                    });
                }

                return Ok(new ApiResponse<ItemAssetResponse>
                {
                    Success = true,
                    Message = "Item asset updated successfully.",
                    Data = result
                });
            }
            finally
            {
                modelStream?.Dispose();
                imageStream?.Dispose();
                audioStream?.Dispose();
            }
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin,Teacher")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var succeeded = await _itemAssetService.DeleteAsync(id);
                if (!succeeded)
                {
                    return NotFound(new ApiResponse
                    {
                        Success = false,
                        Message = "Item asset not found."
                    });
                }

                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Item asset deleted successfully."
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }
    }
}
