using AutoMapper;
using GodotXR.Application.DTOs.Request.Analyze;
using GodotXR.Application.DTOs.Response;
using GodotXR.Application.DTOs.Response.Analyze;
using GodotXR.Domain.Entities;
using GodotXR.Domain.IUnitOfWork;

namespace GodotXR.Application.Services
{
    public class AnalyzeService : IAnalyzeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AnalyzeService(IUnitOfWork unitOfWork, IMapper mapper) 
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedResponse<AnalyzeResponse>> GetListAnalyzeAsync(int pageNumber, int pageSize)
        {
            var paged = await _unitOfWork.AnalyzeRepository.GetPagedAsync(
                pageNumber,
                pageSize,
                r =>  !r.IsDeleted);

            return new PagedResponse<AnalyzeResponse>
            {
                PageNumber = paged.PageNumber,
                PageSize = paged.PageSize,
                TotalCount = paged.TotalCount,
                TotalPages = paged.TotalPages,
                Items = _mapper.Map<List<AnalyzeResponse>>(paged.Items)
            };
        }

        public async Task<AnalyzeResponse?> GetAnalyzeByIdAsync(int id)
        {
            var classroom = await _unitOfWork.AnalyzeRepository
                .GetFirstOrDefaultAsync(
                    filter: c => c.Id == id && !c.IsDeleted,
                    tracked: false);

            return classroom == null ? null : _mapper.Map<AnalyzeResponse>(classroom);
        }

        public async Task<IEnumerable<AnalyzeResponse>> GetAnalyzesByChildIdAsync(int childId)
        {
            var list = await _unitOfWork.AnalyzeRepository.FindAsync(
                filter: r => r.ChildId == childId && !r.IsDeleted,
                orderBy: q => q.OrderByDescending(r => r.CreatedAt),
                tracked: false);

            return _mapper.Map<List<AnalyzeResponse>>(list);
        }

        public async Task<(bool Succeeded, IEnumerable<string> Errors, AnalyzeResponse? Data)> CreateAnalyzeAsync(CreateAnalyzeRequest request)
        {
            var child = await _unitOfWork.ChildProfileRepository.GetFirstOrDefaultAsync(
                filter: c => c.Id == request.ChildId && !c.IsDeleted,
                tracked: false);

            if (child == null)
            {
                return (false, new[] { "Child profile not found." }, null);
            }

            var analyze = _mapper.Map<Analyze>(request);

            await _unitOfWork.AnalyzeRepository.AddAsync(analyze);
            await _unitOfWork.SaveChangesAsync();

            return (true, Enumerable.Empty<string>(), _mapper.Map<AnalyzeResponse>(analyze)
            );
        }

        public async Task<(bool Succeeded, bool NotFound, IEnumerable<string> Errors, AnalyzeResponse? Data)> UpdateAnalyzeAsync(int id, UpdateAnalyzeRequest request)
        {
            var analyze = await _unitOfWork.AnalyzeRepository.GetFirstOrDefaultAsync(
                filter: x => x.Id == id && !x.IsDeleted);

            if (analyze == null)
            {
                return (false, true, new[] { "Analyze not found." }, null
                );
            }

            _mapper.Map(request, analyze);

            _unitOfWork.AnalyzeRepository.Update(analyze);
            await _unitOfWork.SaveChangesAsync();

            return (true, false, Enumerable.Empty<string>(), _mapper.Map<AnalyzeResponse>(analyze));
        }

        public async Task<(bool Succeeded, bool NotFound, IEnumerable<string> Errors)> DeleteAnalyzeAsync(int id)
        {
            var analyze = await _unitOfWork.AnalyzeRepository.GetFirstOrDefaultAsync(
                x => x.Id == id && !x.IsDeleted);

            if (analyze == null)
            {
                return (false, true, new[] { "Analyze not found." });
            }

            analyze.IsDeleted = true;
            analyze.UpdatedAt = DateTime.UtcNow;
            analyze.DeletedAt = DateTime.UtcNow;

            _unitOfWork.AnalyzeRepository.Update(analyze);
            await _unitOfWork.SaveChangesAsync();

            return (true, false, Enumerable.Empty<string>());
        }
    }
}
