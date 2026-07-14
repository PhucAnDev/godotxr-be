using GodotXR.Domain.Entities;

namespace GodotXR.Domain.IRepositories
{
    public interface IEnrollmentRepository : IGenericRepository<Enrollment>
    {
        Task<Enrollment?> GetByIdWithDetailsAsync(int id);
        Task<IEnumerable<Enrollment>> GetAllWithDetailsAsync();
        Task<bool> HasActiveEnrollmentAsync(int childId, int classId, int? excludeId = null);
        Task<IEnumerable<Enrollment>> GetByChildIdAsync(int childId);
        Task<bool> HasTeacherAccessToChildAsync(int teacherId, int childId);
    }
}