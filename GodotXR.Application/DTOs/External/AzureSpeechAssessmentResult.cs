using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Text.Json.Serialization;

namespace GodotXR.Application.DTOs.External
{
    public class AzureSpeechAssessmentResult
    {
        [JsonPropertyName("RecognitionStatus")]
        public string RecognitionStatus { get; set; } = string.Empty;

        [JsonPropertyName("DisplayText")]
        public string? DisplayText { get; set; }

        [JsonPropertyName("NBest")]
        public List<AzureNBestResult> NBest { get; set; } = new();
    }

    public class AzureNBestResult
    {
        [JsonPropertyName("Confidence")]
        public double Confidence { get; set; }

        [JsonPropertyName("Display")]
        public string? Display { get; set; }

        [JsonPropertyName("PronunciationAssessment")]
        public AzurePronunciationScore? PronunciationAssessment { get; set; }

        [JsonPropertyName("Words")]
        public List<AzureWordResult> Words { get; set; } = new();
    }

    public class AzurePronunciationScore
    {
        [JsonPropertyName("AccuracyScore")]
        public double AccuracyScore { get; set; }

        [JsonPropertyName("FluencyScore")]
        public double FluencyScore { get; set; }

        [JsonPropertyName("CompletenessScore")]
        public double CompletenessScore { get; set; }

        [JsonPropertyName("PronScore")]
        public double PronScore { get; set; }
    }

    public class AzureWordResult
    {
        [JsonPropertyName("Word")]
        public string Word { get; set; } = string.Empty;

        [JsonPropertyName("PronunciationAssessment")]
        public AzureWordScore? PronunciationAssessment { get; set; }

        [JsonPropertyName("Phonemes")]
        public List<AzurePhonemeResult> Phonemes { get; set; } = new();
    }

    public class AzureWordScore
    {
        [JsonPropertyName("AccuracyScore")]
        public double AccuracyScore { get; set; }

        // None | Mispronunciation | Omission | Insertion
        [JsonPropertyName("ErrorType")]
        public string ErrorType { get; set; } = "None";
    }

    public class AzurePhonemeResult
    {
        [JsonPropertyName("Phoneme")]
        public string Phoneme { get; set; } = string.Empty;

        [JsonPropertyName("PronunciationAssessment")]
        public AzurePhonemeScore? PronunciationAssessment { get; set; }
    }

    public class AzurePhonemeScore
    {
        [JsonPropertyName("AccuracyScore")]
        public double AccuracyScore { get; set; }
    }
}

