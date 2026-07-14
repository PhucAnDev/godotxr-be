using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace GodotXR.Application.DTOs.Request.Report
{
    public class CreateReportRequest
    {
        [Required]
        public int AnalyzeId { get; set; }

        [Required]
        [MaxLength(50)]
        public string ReportFormat { get; set; } = string.Empty; // e.g. "PDF", "Excel"

        public string? FileUrl { get; set; }
    }
}
