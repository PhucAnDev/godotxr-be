namespace GodotXR.Infrastructure.Configurations
{
    public sealed class StorageOptions
    {
        public const string SectionName = "Storage";

        public string Endpoint { get; set; } = string.Empty;

        public string AccessKey { get; set; } = string.Empty;

        public string SecretKey { get; set; } = string.Empty;

        public string BucketName { get; set; } = string.Empty;

        public bool UseSSL { get; set; }
    }
}
