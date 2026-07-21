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

    public class AudioChunkResponse
    {
        public int ChunkIndex { get; set; }
        public string ChunkUrl { get; set; } = null!;

        public AudioChunkResponse(int chunkIndex, string chunkUrl)
        {
            ChunkIndex = chunkIndex;
            ChunkUrl = chunkUrl;
        }
    }


    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Parent")]
    public class FilesController : ControllerBase
    {
        private readonly IStorageService _storage;
        private readonly IConfiguration _configuration;

        public FilesController(IStorageService storage, IConfiguration configuration)
        {
            _storage = storage;
            _configuration = configuration;
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

        [HttpGet("chunks/{childProfileId}/{sessionId}")]
        public async Task<ActionResult<IEnumerable<AudioChunkResponse>>> GetChunks(
            int childProfileId,
            string sessionId,
            CancellationToken ct)
        {
            var prefix = $"records/{childProfileId}/{sessionId}/chunks/";
            var keys = await _storage.ListObjectsAsync(prefix, ct);

            var result = new List<AudioChunkResponse>();
            foreach (var key in keys)
            {
                var fileName = Path.GetFileNameWithoutExtension(key);
                if (fileName != null && fileName.StartsWith("chunk_") && int.TryParse(fileName.Substring("chunk_".Length), out var index))
                {
                    var chunkUrl = await _storage.GetPresignedUrlAsync(key, 3600, ct);
                    result.Add(new AudioChunkResponse(index, chunkUrl));
                }
            }

            return Ok(result.OrderBy(c => c.ChunkIndex));
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

        [HttpGet("{childProfileId}/{folderId}/DownloadMetadata")]
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

        [HttpGet("{childProfileId}/{folderId}/DownloadAudio")]
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

        [HttpGet("chunks/{childProfileId}/{sessionId}/{chunkIndex}/DownloadChunk")]
        public async Task<IActionResult> DownloadChunk(
            int childProfileId,
            string sessionId,
            int chunkIndex,
            CancellationToken ct)
        {
            var objectName = $"records/{childProfileId}/{sessionId}/chunks/chunk_{chunkIndex}.wav";
            var memoryStream = new MemoryStream();
            try
            {
                await _storage.DownloadAsync(objectName, memoryStream, ct);
                memoryStream.Position = 0;
                return File(memoryStream, "audio/wav", $"chunk_{chunkIndex}.wav");
            }
            catch (Exception)
            {
                return NotFound("Audio chunk not found.");
            }
        }

        [HttpPost("chunks/assess")]
        public async Task<IActionResult> AssessChunk(
            [FromBody] AssessChunkRequest request,
            CancellationToken ct)
        {
            var subKey = _configuration["AzureSpeech:SubscriptionKey"];
            var region = _configuration["AzureSpeech:Region"] ?? "southeastasia";
            var language = _configuration["AzureSpeech:Language"] ?? "vi-VN";

            if (string.IsNullOrEmpty(subKey) || subKey.Contains("YOUR_AZURE_SPEECH_KEY") || subKey == "your-key")
            {
                // Fallback mock assessment result
                var random = new Random();
                var accuracyScore = random.Next(75, 98);
                var pronunciationScore = random.Next(70, 96);
                var fluencyScore = random.Next(65, 95);
                var completenessScore = 100;

                var wordsList = request.ReferenceText.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                var mockWords = new List<object>();
                foreach (var w in wordsList)
                {
                    var wordAccuracy = random.Next(70, 100);
                    var errorType = wordAccuracy < 80 ? "Mispronunciation" : "None";
                    mockWords.Add(new
                    {
                        Word = w,
                        PronunciationAssessment = new
                        {
                            AccuracyScore = wordAccuracy,
                            ErrorType = errorType
                        }
                    });
                }

                var mockResult = new
                {
                    RecognitionStatus = "Success",
                    PronunciationAssessment = new
                    {
                        AccuracyScore = accuracyScore,
                        PronunciationScore = pronunciationScore,
                        CompletenessScore = completenessScore,
                        FluencyScore = fluencyScore
                    },
                    Words = mockWords
                };

                return Ok(mockResult);
            }

            var objectName = $"records/{request.ChildProfileId}/{request.SessionId}/chunks/chunk_{request.ChunkIndex}.wav";
            var memoryStream = new MemoryStream();
            try
            {
                await _storage.DownloadAsync(objectName, memoryStream, ct);
                memoryStream.Position = 0;
            }
            catch (Exception)
            {
                return NotFound("Audio chunk not found in storage.");
            }

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subKey);

            var paramJson = $$"""
            {
              "ReferenceText": "{{request.ReferenceText}}",
              "GradingSystem": "HundredMark",
              "Granularity": "Phoneme",
              "Dimension": "Comprehensive"
            }
            """;
            var paramBytes = System.Text.Encoding.UTF8.GetBytes(paramJson);
            var paramBase64 = Convert.ToBase64String(paramBytes);
            client.DefaultRequestHeaders.Add("Pronunciation-Assessment", paramBase64);

            var content = new StreamContent(memoryStream);
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("audio/wav");

            var url = $"https://{region}.stt.speech.microsoft.com/speech/recognition/conversation/cognitiveservices/v1?language={language}";

            try
            {
                var response = await client.PostAsync(url, content, ct);
                if (!response.IsSuccessStatusCode)
                {
                    return BadRequest($"Azure Speech API error: {response.StatusCode} - {await response.Content.ReadAsStringAsync(ct)}");
                }

                var responseString = await response.Content.ReadAsStringAsync(ct);
                using var doc = System.Text.Json.JsonDocument.Parse(responseString);
                var root = doc.RootElement;
                if (root.TryGetProperty("NBest", out var nbest) && nbest.GetArrayLength() > 0)
                {
                    var best = nbest[0];
                    return Ok(best);
                }

                return Ok(root);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error communicating with Azure: {ex.Message}");
            }
        }
    }

    public class AssessChunkRequest
    {
        [Required]
        public int ChildProfileId { get; set; }

        [Required]
        public string SessionId { get; set; } = null!;

        [Required]
        public int ChunkIndex { get; set; }

        [Required]
        public string ReferenceText { get; set; } = null!;
    }
}
