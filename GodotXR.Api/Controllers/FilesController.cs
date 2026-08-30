using GodotXR.Application.DTOs.Response.FileUpload;
using GodotXR.Application.Helpers;
using GodotXR.Application.Services;
using GodotXR.Domain.Entities;
using GodotXR.Domain.IUnitOfWork;
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
        [FromForm(Name = "childSessionId")]
        public string SessionId { get; set; } = null!;  // Maps to VR client form field 'childSessionId'

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
    [Authorize(Roles = "Admin,Teacher,Parent")]
    public class FilesController : ControllerBase
    {
        private readonly IStorageService _storage;
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;

        public FilesController(IStorageService storage, IConfiguration configuration, IUnitOfWork unitOfWork)
        {
            _storage = storage;
            _configuration = configuration;
            _unitOfWork = unitOfWork;
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

            // Use the SessionId from the VR app as the folder path for consistent linking
            var sessionId = request.SessionId;

            var metadataObject = $"records/{request.ChildProfileId}/{sessionId}/metadata.json";
            var audioObject = $"records/{request.ChildProfileId}/{sessionId}/voice.wav";

            await using var metadataStream = request.Metadata.OpenReadStream();

            await _storage.UploadAsync(metadataStream, metadataObject, "application/json", ct);

            await using var audioStream = request.Audio.OpenReadStream();

            await _storage.UploadAsync(audioStream, audioObject, "audio/wav", ct);

            // Link URLs back to the Result row using the same SessionId
            var dbResult = await _unitOfWork.ResultRepository.GetBySessionIdAsync(sessionId);
            if (dbResult != null)
            {
                var baseUrl = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";
                dbResult.AudioRecordUrl = $"{baseUrl}/api/files/{request.ChildProfileId}/{sessionId}/DownloadAudio";
                dbResult.ReplayDataUrl = $"{baseUrl}/api/files/{request.ChildProfileId}/{sessionId}/DownloadMetadata";

                _unitOfWork.ResultRepository.Update(dbResult);
                await _unitOfWork.SaveChangesAsync();
            }

            return Ok(new UploadFilesResponse(sessionId));
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

            // Extract unique sessionIds from object paths: records/{childId}/{sessionId}/...
            var sessionIds = keys
                .Select(key =>
                {
                    var parts = key.Split('/');
                    // parts[0]=records, parts[1]=childId, parts[2]=sessionId
                    return parts.Length >= 4 ? parts[2] : null;
                })
                .Where(id => !string.IsNullOrEmpty(id))
                .Distinct()
                .ToList();

            var result = new List<FileGroupResponse>();
            foreach (var sid in sessionIds)
            {
                var metadataObject = $"records/{childProfileId}/{sid}/metadata.json";
                var audioObject = $"records/{childProfileId}/{sid}/voice.wav";

                var metadataUrl = await _storage.GetPresignedUrlAsync(metadataObject, 3600, ct);
                var audioUrl = await _storage.GetPresignedUrlAsync(audioObject, 3600, ct);

                result.Add(new FileGroupResponse(sid, metadataUrl, audioUrl));
            }

            return Ok(result);
        }

        [HttpGet("{childProfileId}/{sessionId}")]
        public async Task<ActionResult<FileGroupResponse>> GetById(
            int childProfileId,
            string sessionId,
            CancellationToken ct)
        {
            var prefix = $"records/{childProfileId}/{sessionId}/";
            var keys = await _storage.ListObjectsAsync(prefix, ct);

            if (!keys.Any())
            {
                return NotFound("The specified files do not exist.");
            }

            var metadataObject = $"records/{childProfileId}/{sessionId}/metadata.json";
            var audioObject = $"records/{childProfileId}/{sessionId}/voice.wav";

            var metadataUrl = await _storage.GetPresignedUrlAsync(metadataObject, 3600, ct);
            var audioUrl = await _storage.GetPresignedUrlAsync(audioObject, 3600, ct);

            return Ok(new FileGroupResponse(sessionId, metadataUrl, audioUrl));
        }

        [HttpGet("{childProfileId}/{sessionId}/DownloadMetadata")]
        public async Task<IActionResult> DownloadMetadata(
            int childProfileId,
            string sessionId,
            CancellationToken ct)
        {
            var objectName = $"records/{childProfileId}/{sessionId}/metadata.json";
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

        [HttpGet("{childProfileId}/{sessionId}/DownloadAudio")]
        public async Task<IActionResult> DownloadAudio(
            int childProfileId,
            string sessionId,
            CancellationToken ct)
        {
            var objectName = $"records/{childProfileId}/{sessionId}/voice.wav";
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

            if (string.IsNullOrEmpty(subKey))
            {
                return BadRequest("Azure Speech Subscription Key is not configured on the server.");
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
                    var loadedKeyInfo = string.IsNullOrEmpty(subKey) 
                        ? "null/empty" 
                        : $"Length: {subKey.Length}, Start: {subKey[..Math.Min(4, subKey.Length)]}, End: {subKey[Math.Max(0, subKey.Length - 4)..]}";
                    return BadRequest($"Azure Speech API error: {response.StatusCode} - {await response.Content.ReadAsStringAsync(ct)} | Key Info: {loadedKeyInfo} | Region: {region}");
                }

                var responseString = await response.Content.ReadAsStringAsync(ct);
                var node = System.Text.Json.Nodes.JsonNode.Parse(responseString);
                if (node != null && node["NBest"] is System.Text.Json.Nodes.JsonArray nbest && nbest.Count > 0)
                {
                    var bestItem = nbest[0];

                    // Extract actual spoken text recognized by Azure STT (e.g., "triệu kirkard.")
                    string actualSpokenText = bestItem["Display"]?.ToString() ?? bestItem["Lexical"]?.ToString() ?? node["DisplayText"]?.ToString() ?? string.Empty;

                    // Calculate phrase similarity score between reference text and actual spoken text
                    float phraseSim = PhraseSimilarityHelper.CalculateSimilarity(request.ReferenceText, actualSpokenText);

                    try
                    {
                        var pronAssess = bestItem["PronunciationAssessment"];
                        float rawFluency = pronAssess?["FluencyScore"]?.GetValue<float>() ?? 0f;
                        float rawPron = pronAssess?["PronScore"]?.GetValue<float>() ?? pronAssess?["PronunciationScore"]?.GetValue<float>() ?? 0f;
                        float rawCompleteness = pronAssess?["CompletenessScore"]?.GetValue<float>() ?? 0f;
                        float rawAccuracy = pronAssess?["AccuracyScore"]?.GetValue<float>() ?? 0f;

                        // Calibrate overall scores based on phrase similarity match
                        float calibratedAccuracy = Math.Clamp(rawAccuracy * (0.2f + 0.8f * (phraseSim / 100f)), 0f, 100f);
                        float calibratedPron = Math.Clamp((rawPron * 0.4f) + (phraseSim * 0.6f), 0f, 100f);
                        float calibratedCompleteness = Math.Clamp(rawCompleteness * (phraseSim / 100f), 0f, 100f);
                        float calibratedFluency = rawFluency;

                        // Round scores for clean display
                        calibratedAccuracy = MathF.Round(calibratedAccuracy, 1);
                        calibratedPron = MathF.Round(calibratedPron, 1);
                        calibratedCompleteness = MathF.Round(calibratedCompleteness, 1);
                        calibratedFluency = MathF.Round(calibratedFluency, 1);

                        // Update JSON response bestItem["PronunciationAssessment"] fields
                        if (pronAssess is System.Text.Json.Nodes.JsonObject pronObj)
                        {
                            pronObj["AccuracyScore"] = calibratedAccuracy;
                            pronObj["PronScore"] = calibratedPron;
                            pronObj["PronunciationScore"] = calibratedPron;
                            pronObj["CompletenessScore"] = calibratedCompleteness;
                            pronObj["FluencyScore"] = calibratedFluency;
                        }

                        if (bestItem["Words"] is System.Text.Json.Nodes.JsonArray wordsArray)
                        {
                            // Soft delete existing speech accuracy records for this chunk to prevent duplicate rows
                            var existingRecords = await _unitOfWork.ChildSpeechAccuracyRepository.GetByChunkAsync(
                                request.ChildProfileId,
                                request.SessionId,
                                request.ChunkIndex);

                            foreach (var existing in existingRecords)
                            {
                                existing.IsDeleted = true;
                                existing.DeletedAt = DateTime.UtcNow;
                            }

                            foreach (var wNode in wordsArray)
                            {
                                var wordStr = wNode["Word"]?.ToString() ?? string.Empty;
                                if (string.IsNullOrWhiteSpace(wordStr)) continue;

                                var wAssess = wNode["PronunciationAssessment"];
                                float wAcc = wAssess?["AccuracyScore"]?.GetValue<float>() ?? 0f;
                                string? wErr = wAssess?["ErrorType"]?.ToString();

                                // Calibrate word accuracy score if phrase similarity is low
                                float calibratedWordAcc = Math.Clamp(wAcc * (0.3f + 0.7f * (phraseSim / 100f)), 0f, 100f);
                                calibratedWordAcc = MathF.Round(calibratedWordAcc, 1);

                                if (phraseSim < 40f && (wErr == "None" || string.IsNullOrEmpty(wErr)))
                                {
                                    wErr = "Mispronunciation";
                                }

                                if (wAssess is System.Text.Json.Nodes.JsonObject wAssessObj)
                                {
                                    wAssessObj["AccuracyScore"] = calibratedWordAcc;
                                    if (wErr != null) wAssessObj["ErrorType"] = wErr;
                                }

                                var accuracyEntity = new ChildSpeechAccuracy
                                {
                                    ChildProfileId = request.ChildProfileId,
                                    SessionId = request.SessionId,
                                    AudioChunkIndex = request.ChunkIndex,
                                    Word = wordStr,
                                    AccuracyScore = calibratedWordAcc,
                                    FluencyScore = calibratedFluency,
                                    PronunciationScore = calibratedPron,
                                    CompletenessScore = calibratedCompleteness,
                                    ErrorType = wErr,
                                    CreatedAt = DateTime.UtcNow
                                };

                                await _unitOfWork.ChildSpeechAccuracyRepository.AddAsync(accuracyEntity);
                            }
                            await _unitOfWork.SaveChangesAsync();
                        }
                    }
                    catch (Exception)
                    {
                        // Ignore DB save errors so API assessment response is not interrupted
                    }

                    return Ok(bestItem);
                }

                return Ok(node);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error communicating with Azure: {ex.Message}");
            }
        }

        [HttpGet("assets/{assetId:int}/model")]
        [AllowAnonymous]
        public async Task<IActionResult> DownloadAssetModel(int assetId, CancellationToken ct)
        {
            var asset = await _unitOfWork.Repository<ItemAsset>().GetByIdAsync(assetId);
            if (asset == null || asset.IsDeleted || string.IsNullOrEmpty(asset.ModelUrl))
            {
                return NotFound("Model asset not found.");
            }

            var memoryStream = new MemoryStream();
            try
            {
                await _storage.DownloadAsync(asset.ModelUrl, memoryStream, ct);
                memoryStream.Position = 0;
                var fileName = Path.GetFileName(asset.ModelUrl) ?? "model.glb";
                return File(memoryStream, "application/octet-stream", fileName);
            }
            catch (Exception)
            {
                return NotFound("Model file not found in storage.");
            }
        }

        [HttpGet("assets/{assetId:int}/image")]
        [AllowAnonymous]
        public async Task<IActionResult> DownloadAssetImage(int assetId, CancellationToken ct)
        {
            var asset = await _unitOfWork.Repository<ItemAsset>().GetByIdAsync(assetId);
            if (asset == null || asset.IsDeleted || string.IsNullOrEmpty(asset.ImageUrl))
            {
                return NotFound("Image asset not found.");
            }

            var memoryStream = new MemoryStream();
            try
            {
                await _storage.DownloadAsync(asset.ImageUrl, memoryStream, ct);
                memoryStream.Position = 0;
                var contentType = GetContentType(asset.ImageUrl);
                return File(memoryStream, contentType);
            }
            catch (Exception)
            {
                return NotFound("Image file not found in storage.");
            }
        }

        [HttpGet("assets/{assetId:int}/audio")]
        [AllowAnonymous]
        public async Task<IActionResult> DownloadAssetAudio(int assetId, CancellationToken ct)
        {
            var asset = await _unitOfWork.Repository<ItemAsset>().GetByIdAsync(assetId);
            if (asset == null || asset.IsDeleted || string.IsNullOrEmpty(asset.AudioUrl))
            {
                return NotFound("Audio asset not found.");
            }

            var memoryStream = new MemoryStream();
            try
            {
                await _storage.DownloadAsync(asset.AudioUrl, memoryStream, ct);
                memoryStream.Position = 0;
                var contentType = GetContentType(asset.AudioUrl);
                return File(memoryStream, contentType);
            }
            catch (Exception)
            {
                return NotFound("Audio file not found in storage.");
            }
        }

        [HttpGet("lessons/{lessonId:int}/images/{imageId:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> DownloadLessonImage(int lessonId, int imageId, CancellationToken ct)
        {
            var lessonImage = await _unitOfWork.Repository<LessonImage>().GetByIdAsync(imageId);
            if (lessonImage == null || lessonImage.IsDeleted || lessonImage.LessonId != lessonId || string.IsNullOrEmpty(lessonImage.ImageUrl))
            {
                return NotFound("Lesson image not found.");
            }

            var memoryStream = new MemoryStream();
            try
            {
                await _storage.DownloadAsync(lessonImage.ImageUrl, memoryStream, ct);
                memoryStream.Position = 0;
                var contentType = GetContentType(lessonImage.ImageUrl);
                return File(memoryStream, contentType);
            }
            catch (Exception)
            {
                return NotFound("Lesson image file not found in storage.");
            }
        }

        private static string GetContentType(string path)
        {
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return ext switch
            {
                ".png" => "image/png",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".gif" => "image/gif",
                ".mp3" => "audio/mpeg",
                ".wav" => "audio/wav",
                ".ogg" => "audio/ogg",
                _ => "application/octet-stream"
            };
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
