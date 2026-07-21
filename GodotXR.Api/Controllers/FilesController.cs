using GodotXR.Application.DTOs.Response.FileUpload;
using GodotXR.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace GodotXR.Api.Controllers
{
    public class UploadFilesRequest
    {

        [Required]
        public int ChildProfileId { get; set; }

        [Required]
        public IFormFile Metadata { get; set; } = null!;

        [Required]
        public IFormFile Audio { get; set; } = null!;
    }

    public class UploadAudioChunkRequest
    {
        [Required]
        public int ChildProfileId { get; set; }

        [Required]
        public string SessionId { get; set; } = null!;

        [Required]
        public int ChunkIndex { get; set; }

        [Required]
        public IFormFile AudioFile { get; set; } = null!;

        [Required]
        public bool IsFinalChunk { get; set; }
    }

    public class UploadAudioChunkResponse
    {
        public string Status { get; set; }
        public int ChunkIndex { get; set; }
        public string? VoiceUrl { get; set; }

        public UploadAudioChunkResponse(string status, int chunkIndex, string? voiceUrl = null)
        {
            Status = status;
            ChunkIndex = chunkIndex;
            VoiceUrl = voiceUrl;
        }
    }

    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Parent")]
    public class FilesController : ControllerBase
    {
        private readonly IStorageService _storage;

        public FilesController(IStorageService storage)
        {
            _storage = storage;
        }

        [HttpPost]
        public async Task<ActionResult<UploadFilesResponse>> Upload(
            [FromForm] UploadFilesRequest request,
            CancellationToken ct)
        {
            if (request.Metadata is null || request.Metadata.Length == 0)
            {
                return BadRequest("Metadata file is required.");
            }

            if (request.Audio is null || request.Audio.Length == 0)
            {
                return BadRequest("Audio file is required.");
            }

            var folderId = Guid.NewGuid();

            var metadataObject = $"records/{request.ChildProfileId}/{folderId}/metadata.json";
            var audioObject = $"records/{request.ChildProfileId}/{folderId}/voice.wav";

            await using var metadataStream = request.Metadata.OpenReadStream();

            await _storage.UploadAsync(metadataStream, metadataObject, "application/json", ct);

            await using var audioStream = request.Audio.OpenReadStream();

            await _storage.UploadAsync(audioStream, audioObject, "audio/wav", ct);

            return Ok(new UploadFilesResponse(folderId));
        }

        [HttpPost("chunks")]
        public async Task<ActionResult<UploadAudioChunkResponse>> UploadChunk(
            [FromForm] UploadAudioChunkRequest request,
            CancellationToken ct)
        {
            if (request.AudioFile is null || request.AudioFile.Length == 0)
            {
                return BadRequest("Audio file chunk is required.");
            }

            var chunkObject = $"records/{request.ChildProfileId}/{request.SessionId}/chunks/chunk_{request.ChunkIndex}.wav";

            await using var chunkStream = request.AudioFile.OpenReadStream();
            await _storage.UploadAsync(chunkStream, chunkObject, "audio/wav", ct);

            var chunkUrl = await _storage.GetPresignedUrlAsync(chunkObject, 3600, ct);

            if (request.IsFinalChunk)
            {
                return Ok(new UploadAudioChunkResponse("Completed", request.ChunkIndex, chunkUrl));
            }

            return Ok(new UploadAudioChunkResponse("ChunkUploaded", request.ChunkIndex, chunkUrl));
        }

        [HttpGet("{childProfileId}")]
        public async Task<ActionResult<IEnumerable<FileGroupResponse>>> GetByChildProfile(
            int childProfileId,
            CancellationToken ct)
        {
            var prefix = $"records/{childProfileId}/";
            var keys = await _storage.ListObjectsAsync(prefix, ct);

            var folderIds = keys
                .Select(key =>
                {
                    var parts = key.Split('/');
                    if (parts.Length >= 4 && Guid.TryParse(parts[2], out var folderId))
                    {
                        return (Guid?)folderId;
                    }
                    return null;
                })
                .Where(id => id.HasValue)
                .Select(id => id!.Value)
                .Distinct()
                .ToList();

            var result = new List<FileGroupResponse>();
            foreach (var folderId in folderIds)
            {
                var metadataObject = $"records/{childProfileId}/{folderId}/metadata.json";
                var audioObject = $"records/{childProfileId}/{folderId}/voice.wav";

                var metadataUrl = await _storage.GetPresignedUrlAsync(metadataObject, 3600, ct);
                var audioUrl = await _storage.GetPresignedUrlAsync(audioObject, 3600, ct);

                result.Add(new FileGroupResponse(folderId, metadataUrl, audioUrl));
            }

            return Ok(result);
        }

        [HttpGet("{childProfileId}/{folderId}")]
        public async Task<ActionResult<FileGroupResponse>> GetById(
            int childProfileId,
            Guid folderId,
            CancellationToken ct)
        {
            var prefix = $"records/{childProfileId}/{folderId}/";
            var keys = await _storage.ListObjectsAsync(prefix, ct);

            if (!keys.Any())
            {
                return NotFound("The specified files do not exist.");
            }

            var metadataObject = $"records/{childProfileId}/{folderId}/metadata.json";
            var audioObject = $"records/{childProfileId}/{folderId}/voice.wav";

            var metadataUrl = await _storage.GetPresignedUrlAsync(metadataObject, 3600, ct);
            var audioUrl = await _storage.GetPresignedUrlAsync(audioObject, 3600, ct);

            return Ok(new FileGroupResponse(folderId, metadataUrl, audioUrl));
        }

        [HttpGet("{childProfileId}/{folderId}/metadata")]
        public async Task<IActionResult> DownloadMetadata(
            int childProfileId,
            Guid folderId,
            CancellationToken ct)
        {
            var objectName = $"records/{childProfileId}/{folderId}/metadata.json";
            var memoryStream = new MemoryStream();
            try
            {
                await _storage.DownloadAsync(objectName, memoryStream, ct);
                memoryStream.Position = 0;
                return File(memoryStream, "application/json", "metadata.json");
            }
            catch (Exception)
            {
                return NotFound("Metadata file not found.");
            }
        }

        [HttpGet("{childProfileId}/{folderId}/audio")]
        public async Task<IActionResult> DownloadAudio(
            int childProfileId,
            Guid folderId,
            CancellationToken ct)
        {
            var objectName = $"records/{childProfileId}/{folderId}/voice.wav";
            var memoryStream = new MemoryStream();
            try
            {
                await _storage.DownloadAsync(objectName, memoryStream, ct);
                memoryStream.Position = 0;
                return File(memoryStream, "audio/wav", "voice.wav");
            }
            catch (Exception)
            {
                return NotFound("Audio file not found.");
            }
        }
    }
}
