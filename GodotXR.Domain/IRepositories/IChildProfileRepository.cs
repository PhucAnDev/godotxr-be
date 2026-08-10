using GodotXR.Domain.Entities;

namespace GodotXR.Domain.IRepositories
{
    /// <summary>
    /// Internal DTO: truyền kết quả JOIN Enrollment + Classroom lên Service layer.
    /// </summary>
    public record ChildProfileWithClassDto(
        ChildProfile Child,
        int ClassroomId,
        string ClassroomName,
        string EnrollmentStatus
    );

    public interface IChildProfileRepository : IGenericRepository<ChildProfile>
    {
        /// <summary>
        /// Lấy danh sách học sinh (Active enrollment) trong lớp mà teacher quản lý.
        /// Business rule: 1 teacher chỉ quản lý 1 lớp.
        /// </summary>
        Task<(IEnumerable<ChildProfileWithClassDto> Items, int TotalCount)>
            GetStudentsByTeacherIdAsync(int teacherId, int pageNumber, int pageSize);
    }
}
