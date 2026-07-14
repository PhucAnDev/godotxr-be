namespace GodotXR.Application.Services
{
    public interface IStorageService
    {
        Task<string> UploadAsync(
        Stream stream,
        string fileName,
        string contentType,
        CancellationToken ct);

        Task<string> GetPresignedUrlAsync(
            string objectName,
            int expirySeconds,
            CancellationToken ct);

        Task DeleteAsync(
            string objectName,
            CancellationToken ct);

        Task<IEnumerable<string>> ListObjectsAsync(
            string prefix,
            CancellationToken ct);

        Task DownloadAsync(
            string objectName,
            Stream destinationStream,
            CancellationToken ct);
    }
}
