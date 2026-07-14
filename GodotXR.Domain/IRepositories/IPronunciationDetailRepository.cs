using GodotXR.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodotXR.Domain.IRepositories
{
    public interface IPronunciationDetailRepository : IGenericRepository<PronunciationDetail>
    {
        Task<IEnumerable<PronunciationDetail>> GetByResultIdAsync(int resultId);
    }
}
