using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GodotXR.Application.DTOs.Request.Report;
using GodotXR.Application.DTOs.Response;
using GodotXR.Application.DTOs.Response.Report;

namespace GodotXR.Application.Services
{
    public interface IReportService
    {
        Task<PagedResponse<ReportResponse>> GetListReportAsync(int pageNumber, int pageSize, int userId, string role);
        Task<ReportResponse?> GetReportByIdAsync(int id);
        Task<(bool Succeeded, IEnumerable<string> Errors, ReportResponse? Data)> CreateReportAsync(int generatedBy, CreateReportRequest request);
        Task<(bool Succeeded, bool NotFound, IEnumerable<string> Errors, ReportResponse? Data)> UpdateReportAsync(int id, UpdateReportRequest request);
        Task<(bool Succeeded, bool NotFound, IEnumerable<string> Errors)> DeleteReportAsync(int id);
    }
}