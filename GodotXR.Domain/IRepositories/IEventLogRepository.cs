using GodotXR.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodotXR.Domain.IRepositories
{
    public interface IEventLogRepository : IGenericRepository<EventLog>
    {
        Task<IEnumerable<EventLog>> GetByResultIdAsync(int resultId);
        Task<IEnumerable<EventLog>> GetByChildIdAsync(int childId);
    }
}
