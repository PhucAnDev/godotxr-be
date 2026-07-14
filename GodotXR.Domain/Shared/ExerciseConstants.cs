using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodotXR.Domain.Shared
{
    public static class ExerciseConstants
    {
        public static readonly IReadOnlyCollection<string> AllowedDifficultyLevels =
            new[] { "Easy", "Medium", "Hard" };

        public static readonly IReadOnlyCollection<string> AllowedStatuses =
            new[] { "Active", "Inactive" };

        public static readonly IReadOnlyCollection<string> AllowedAudioExtensions =
            new[] { ".mp3", ".wav", ".ogg", ".m4a" };

        public static readonly IReadOnlyCollection<string> AllowedImageExtensions =
            new[] { ".jpg", ".jpeg", ".png", ".webp", ".gif" };
    }
}