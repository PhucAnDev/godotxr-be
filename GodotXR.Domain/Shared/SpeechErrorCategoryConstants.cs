namespace GodotXR.Domain.Shared
{
    public static class SpeechErrorCategoryConstants
    {
        public const string Substitution = "Thay thế âm";
        public const string Omission = "Nuốt âm/Bỏ sót âm";
        public const string Distortion = "Méo tiếng/Chưa tròn vành rõ chữ";
        public const string ToneShift = "Lệch thanh điệu (Hỏi/Ngã)";

        public const string Default = Substitution;

        public static readonly string[] AllowedValues = new[]
        {
            Substitution,
            Omission,
            Distortion,
            ToneShift
        };

        public static bool IsValid(string? value)
        {
            return !string.IsNullOrWhiteSpace(value) && AllowedValues.Contains(value);
        }

        public static string GetValidOrDefault(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return Default;
            }

            var trimmed = value.Trim();
            return AllowedValues.Contains(trimmed) ? trimmed : Default;
        }
    }
}
