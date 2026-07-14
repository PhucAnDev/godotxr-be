using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace GodotXR.Application.DTOs.Request.Report
{
    public class UpdateReportRequest
    {
        [MaxLength(50)]
        public string? ReportFormat { get; set; }

        public string? FileUrl { get; set; }
    }
}