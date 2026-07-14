using GodotXR.Application.DTOs.Request.Enrollment;
using GodotXR.Application.DTOs.Response;
using GodotXR.Application.DTOs.Response.Enrollment;

namespace GodotXR.Application.Services
{
    public interface IEnrollmentService
    {
        Task<PagedResponse<EnrollmentResponse>> GetListEnrollmentAsync(
            int pageNumber, int pageSize,
            string? status = null,
            int? classId = null,
            string? learningLevel = null);
        Task<EnrollmentResponse?> GetEnrollmentByIdAsync(int id);
        Task<(bool Succeeded, IEnumerable<string> Errors, EnrollmentResponse? Data)>
            CreateEnrollmentAsync(CreateEnrollmentRequest request, int requesterId, string requesterRole);
        Task<(bool Succeeded, bool NotFound, IEnumerable<string> Errors, EnrollmentResponse? Data)>
            UpdateEnrollmentAsync(int id, UpdateEnrollmentRequest request, int requesterId, string requesterRole);
        Task<(bool Succeeded, bool NotFound, IEnumerable<string> Errors, EnrollmentResponse? Data)>
            TransferEnrollmentAsync(int id, TransferEnrollmentRequest request, int requesterId, string requesterRole);
        Task<(bool Succeeded, bool NotFound, IEnumerable<string> Errors, EnrollmentResponse? Data)>
            ApproveEnrollmentAsync(int id, int requesterId, string requesterRole);
        Task<(bool Succeeded, bool NotFound, IEnumerable<string> Errors)>
            DeleteEnrollmentAsync(int id);
        Task<IEnumerable<EnrollmentResponse>> GetEnrollmentsByChildIdAsync(int childId);
    }
}