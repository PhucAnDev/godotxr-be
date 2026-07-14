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
    public class PronunciationDetailRepository : GenericRepository<PronunciationDetail>, IPronunciationDetailRepository
    {
        public PronunciationDetailRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<PronunciationDetail>> GetByResultIdAsync(int resultId)
            => await _context.PronunciationDetails
                .Where(p => p.ResultId == resultId && !p.IsDeleted)
                .ToListAsync();
    }
}
