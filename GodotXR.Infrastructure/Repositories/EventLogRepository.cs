using GodotXR.Domain.Entities;
using GodotXR.Domain.IRepositories;
using GodotXR.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodotXR.Infrastructure.Repositories
{
    public class EventLogRepository : GenericRepository<EventLog>, IEventLogRepository
    {
        public EventLogRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<EventLog>> GetByResultIdAsync(int resultId)
            => await _context.EventLogs
                .Where(e => e.ResultId == resultId && !e.IsDeleted)
                .ToListAsync();

        public async Task<IEnumerable<EventLog>> GetByChildIdAsync(int childId)
            => await _context.EventLogs
                .Where(e => e.ChildId == childId && !e.IsDeleted)
                .OrderByDescending(e => e.EventTime)
                .ToListAsync();
    }
}
