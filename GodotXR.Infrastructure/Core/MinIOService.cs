using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;
using GodotXR.Application.Services;
using GodotXR.Infrastructure.Configurations;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;

namespace GodotXR.Infrastructure.Core
{
    public sealed class MinIOService : IStorageService
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private readonly IMinioClient? _client;
        private readonly StorageOptions _options;

        public MinIOService(
            IOptions<StorageOptions> options,
            IMinioClient? client = null)
        {
            _client = client;
            _options = options.Value;
        }

        private bool IsSupabase => !string.IsNullOrEmpty(_options.Endpoint) && _options.Endpoint.Contains("supabase.co");

        public async Task<string> UploadAsync(
            Stream stream,
            string fileName,
            string contentType,
            CancellationToken ct)
        {
            if (IsSupabase)
            {
                byte[] payloadBytes;
                if (stream is MemoryStream ms)
                {
                    payloadBytes = ms.ToArray();
                }
                else
                {
                    using var tempMs = new MemoryStream();
                    await stream.CopyToAsync(tempMs, ct);
                    payloadBytes = tempMs.ToArray();
                }

                var host = CleanHost(_options.Endpoint);
                var path = $"/storage/v1/s3/{_options.BucketName}/{fileName}";
                var req = CreateSignedRequest(HttpMethod.Put, host, path, string.Empty, _options.Region, _options.AccessKey, _options.SecretKey, payloadBytes, contentType);

                var response = await _httpClient.SendAsync(req, ct);
                response.EnsureSuccessStatusCode();

                return fileName;
            }

            if (_client == null) throw new InvalidOperationException("MinioClient is not configured.");

            var exists = await _client.BucketExistsAsync(
                new BucketExistsArgs()
                    .WithBucket(_options.BucketName),
                ct);

            if (!exists)
            {
                await _client.MakeBucketAsync(
                    new MakeBucketArgs()
                        .WithBucket(_options.BucketName),
                    ct);
            }

            await _client.PutObjectAsync(
                new PutObjectArgs()
                    .WithBucket(_options.BucketName)
                    .WithObject(fileName)
                    .WithStreamData(stream)
                    .WithObjectSize(stream.Length)
                    .WithContentType(contentType),
                ct);

            return fileName;
        }

        public async Task<string> GetPresignedUrlAsync(
            string objectName,
            int expirySeconds,
            CancellationToken ct)
        {
            if (IsSupabase)
            {
                var host = CleanHost(_options.Endpoint);
                // Return fast direct public URL for Supabase storage
                return await Task.FromResult($"https://{host}/storage/v1/object/public/{_options.BucketName}/{objectName}");
            }

            if (_client == null) throw new InvalidOperationException("MinioClient is not configured.");

            return await _client.PresignedGetObjectAsync(
                new PresignedGetObjectArgs()
                    .WithBucket(_options.BucketName)
                    .WithObject(objectName)
                    .WithExpiry(expirySeconds));
        }

        public async Task DeleteAsync(
            string objectName,
            CancellationToken ct)
        {
            if (IsSupabase)
            {
                var host = CleanHost(_options.Endpoint);
                var path = $"/storage/v1/s3/{_options.BucketName}/{objectName}";
                var req = CreateSignedRequest(HttpMethod.Delete, host, path, string.Empty, _options.Region, _options.AccessKey, _options.SecretKey);

                var response = await _httpClient.SendAsync(req, ct);
                if (!response.IsSuccessStatusCode && response.StatusCode != System.Net.HttpStatusCode.NotFound)
                {
                    response.EnsureSuccessStatusCode();
                }
                return;
            }

            if (_client == null) throw new InvalidOperationException("MinioClient is not configured.");

            await _client.RemoveObjectAsync(
                new RemoveObjectArgs()
                    .WithBucket(_options.BucketName)
                    .WithObject(objectName),
                ct);
        }

        public async Task<IEnumerable<string>> ListObjectsAsync(
            string prefix,
            CancellationToken ct)
        {
            if (IsSupabase)
            {
                var host = CleanHost(_options.Endpoint);
                var path = $"/storage/v1/s3/{_options.BucketName}";
                var queryString = string.IsNullOrEmpty(prefix) ? string.Empty : $"prefix={Uri.EscapeDataString(prefix)}";
                var req = CreateSignedRequest(HttpMethod.Get, host, path, queryString, _options.Region, _options.AccessKey, _options.SecretKey);

                var response = await _httpClient.SendAsync(req, ct);
                response.EnsureSuccessStatusCode();

                var xml = await response.Content.ReadAsStringAsync(ct);
                var doc = XDocument.Parse(xml);
                XNamespace ns = doc.Root?.GetDefaultNamespace() ?? XNamespace.None;

                var list = new List<string>();
                if (doc.Root != null)
                {
                    foreach (var element in doc.Root.Elements(ns + "Contents"))
                    {
                        var key = element.Element(ns + "Key")?.Value;
                        if (!string.IsNullOrEmpty(key))
                        {
                            list.Add(key);
                        }
                    }
                }

                return list;
            }

            if (_client == null) throw new InvalidOperationException("MinioClient is not configured.");

            var listArgs = new ListObjectsArgs()
                .WithBucket(_options.BucketName)
                .WithPrefix(prefix)
                .WithRecursive(true);

            var items = new List<string>();

            await foreach (var item in _client.ListObjectsEnumAsync(listArgs).WithCancellation(ct))
            {
                items.Add(item.Key);
            }

            return items;
        }

        public async Task DownloadAsync(
            string objectName,
            Stream destinationStream,
            CancellationToken ct)
        {
            if (IsSupabase)
            {
                var host = CleanHost(_options.Endpoint);
                var publicUrl = $"https://{host}/storage/v1/object/public/{_options.BucketName}/{objectName}";
                var response = await _httpClient.GetAsync(publicUrl, ct);

                if (!response.IsSuccessStatusCode)
                {
                    // Fallback to signed GET request
                    var path = $"/storage/v1/s3/{_options.BucketName}/{objectName}";
                    var req = CreateSignedRequest(HttpMethod.Get, host, path, string.Empty, _options.Region, _options.AccessKey, _options.SecretKey);
                    response = await _httpClient.SendAsync(req, ct);
                    response.EnsureSuccessStatusCode();
                }

                await response.Content.CopyToAsync(destinationStream, ct);
                return;
            }

            if (_client == null) throw new InvalidOperationException("MinioClient is not configured.");

            var args = new GetObjectArgs()
                .WithBucket(_options.BucketName)
                .WithObject(objectName)
                .WithCallbackStream((stream) =>
                {
                    stream.CopyTo(destinationStream);
                });

            await _client.GetObjectAsync(args, ct);
        }

        private static string CleanHost(string endpoint)
        {
            var host = endpoint.Replace("https://", string.Empty).Replace("http://", string.Empty);
            var slashIdx = host.IndexOf('/');
            if (slashIdx >= 0)
            {
                host = host.Substring(0, slashIdx);
            }
            return host.TrimEnd('/');
        }

        private static HttpRequestMessage CreateSignedRequest(
            HttpMethod method,
            string host,
            string path,
            string queryString,
            string region,
            string accessKey,
            string secretKey,
            byte[]? payloadBytes = null,
            string? contentType = null)
        {
            payloadBytes ??= Array.Empty<byte>();
            var now = DateTime.UtcNow;
            var amzDate = now.ToString("yyyyMMddTHHmmssZ");
            var dateStamp = now.ToString("yyyyMMdd");
            var payloadHash = Sha256Hex(payloadBytes);

            var canonicalHeaders = $"host:{host}\nx-amz-content-sha256:{payloadHash}\nx-amz-date:{amzDate}\n";
            var signedHeaders = "host;x-amz-content-sha256;x-amz-date";
            var canonicalRequest = $"{method.Method}\n{path}\n{queryString}\n{canonicalHeaders}\n{signedHeaders}\n{payloadHash}";

            var algorithm = "AWS4-HMAC-SHA256";
            var credentialScope = $"{dateStamp}/{region}/s3/aws4_request";
            var stringToSign = $"{algorithm}\n{amzDate}\n{credentialScope}\n{Sha256Hex(Encoding.UTF8.GetBytes(canonicalRequest))}";

            var kDate = HmacSHA256(Encoding.UTF8.GetBytes("AWS4" + secretKey), dateStamp);
            var kRegion = HmacSHA256(kDate, region);
            var kService = HmacSHA256(kRegion, "s3");
            var kSigning = HmacSHA256(kService, "aws4_request");
            var signature = ToHex(HmacSHA256(kSigning, stringToSign));

            var authHeader = $"{algorithm} Credential={accessKey}/{credentialScope}, SignedHeaders={signedHeaders}, Signature={signature}";

            var fullUri = string.IsNullOrEmpty(queryString) ? $"https://{host}{path}" : $"https://{host}{path}?{queryString}";
            var request = new HttpRequestMessage(method, fullUri);
            if (payloadBytes.Length > 0)
            {
                request.Content = new ByteArrayContent(payloadBytes);
                if (!string.IsNullOrEmpty(contentType))
                {
                    request.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(contentType);
                }
            }
            request.Headers.Host = host;
            request.Headers.Add("x-amz-date", amzDate);
            request.Headers.Add("x-amz-content-sha256", payloadHash);
            request.Headers.TryAddWithoutValidation("Authorization", authHeader);

            return request;
        }

        private static byte[] HmacSHA256(byte[] key, string data)
        {
            using var hmac = new HMACSHA256(key);
            return hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
        }

        private static string ToHex(byte[] bytes)
        {
            var sb = new StringBuilder(bytes.Length * 2);
            foreach (var b in bytes) sb.Append(b.ToString("x2"));
            return sb.ToString();
        }

        private static string Sha256Hex(byte[] bytes)
        {
            using var sha = SHA256.Create();
            return ToHex(sha.ComputeHash(bytes));
        }
    }
}
