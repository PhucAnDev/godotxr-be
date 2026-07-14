using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodotXR.Application.DTOs.Response.Report
{
    public class ReportResponse
    {
        public int Id { get; set; }
        public int AnalyzeId { get; set; }
        public int GeneratedBy { get; set; }
        public string GeneratedByName { get; set; } = string.Empty;
        public string ReportFormat { get; set; } = string.Empty;
        public string? FileUrl { get; set; }
        public DateTime GeneratedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

}
