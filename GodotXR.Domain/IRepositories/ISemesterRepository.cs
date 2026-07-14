using GodotXR.Domain.Entities;

namespace GodotXR.Domain.IRepositories
{
    public interface ISemesterRepository : IGenericRepository<Semester>
    {
        Task<bool> HasOverlappingAsync(int schoolYearId, DateTime startDate, DateTime endDate, int? excludeId = null);
    }
}