using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace GodotXR.Application.Helpers
{
    public static class PhraseSimilarityHelper
    {
        /// <summary>
        /// Calculates similarity percentage (0 - 100) between expected reference text and actual spoken text.
        /// </summary>
        public static float CalculateSimilarity(string expectedText, string actualSpokenText)
        {
            if (string.IsNullOrWhiteSpace(expectedText)) return 0f;
            if (string.IsNullOrWhiteSpace(actualSpokenText)) return 0f;

            string normalizedExpected = NormalizeText(expectedText);
            string normalizedActual = NormalizeText(actualSpokenText);

            if (string.IsNullOrEmpty(normalizedExpected) || string.IsNullOrEmpty(normalizedActual)) return 0f;
            if (normalizedExpected == normalizedActual) return 100f;

            // 1. Levenshtein Character-level Similarity
            int distance = LevenshteinDistance(normalizedExpected, normalizedActual);
            int maxLength = Math.Max(normalizedExpected.Length, normalizedActual.Length);
            float charSim = maxLength == 0 ? 100f : (1f - ((float)distance / maxLength)) * 100f;

            // 2. Word-level Overlap Similarity (Token Match)
            var expectedWords = normalizedExpected.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var actualWords = normalizedActual.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (expectedWords.Length == 0) return 0f;

            int matchCount = 0;
            foreach (var word in expectedWords)
            {
                if (actualWords.Contains(word))
                {
                    matchCount++;
                }
            }

            float wordOverlapSim = ((float)matchCount / expectedWords.Length) * 100f;

            // Weighted Combined Score: 60% Word Overlap + 40% Character Similarity
            float combinedSim = (wordOverlapSim * 0.6f) + (charSim * 0.4f);
            return Math.Clamp(combinedSim, 0f, 100f);
        }

        private static string NormalizeText(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return string.Empty;
            string cleaned = text.ToLowerInvariant().Trim();
            // Remove punctuation marks
            cleaned = Regex.Replace(cleaned, @"[^\w\s]", "");
            // Replace multiple spaces with a single space
            return Regex.Replace(cleaned, @"\s+", " ").Trim();
        }

        private static int LevenshteinDistance(string s, string t)
        {
            int n = s.Length;
            int m = t.Length;
            int[,] d = new int[n + 1, m + 1];

            if (n == 0) return m;
            if (m == 0) return n;

            for (int i = 0; i <= n; d[i, 0] = i++) { }
            for (int j = 0; j <= m; d[0, j] = j++) { }

            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= m; j++)
                {
                    int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;
                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }
            return d[n, m];
        }
    }
}
