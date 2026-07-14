using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodotXR.Application.DTOs.Request.Result
{
    public class SubmitPronunciationDetailRequest
    {
        public string ExpectedPhoneme { get; set; } = null!;  
        public string ActualPhoneme { get; set; } = null!;    
        public int AccuracyScore { get; set; }                
        public string? IssueType { get; set; }                
        public string? ReplayDataUrl { get; set; }
    }
}
