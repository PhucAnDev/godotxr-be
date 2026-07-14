using AutoMapper;
using GodotXR.Application.DTOs.Request.Program;
using GodotXR.Application.DTOs.Response;
using GodotXR.Application.DTOs.Response.Program;
using GodotXR.Domain.Entities;
using GodotXR.Domain.IUnitOfWork;

namespace GodotXR.Application.Services
{
    public class ProgramService : IProgramService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProgramService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedResponse<ProgramResponse>> GetListProgramAsync(int pageNumber, int pageSize)
        {
            var paged = await _unitOfWork.ProgramRepository.GetPagedAsync(pageNumber, pageSize);

            return new PagedResponse<ProgramResponse>
            {
                PageNumber = paged.PageNumber,
                PageSize = paged.PageSize,
                TotalCount = paged.TotalCount,
                TotalPages = paged.TotalPages,
                Items = _mapper.Map<List<ProgramResponse>>(paged.Items)
            };
        }

        public async Task<ProgramResponse?> GetProgramByIdAsync(int id)
        {
            var program = await _unitOfWork.ProgramRepository
                .GetFirstOrDefaultAsync(
                    filter: p => p.Id == id && !p.IsDeleted,
                    includeProperties: "Lessons",
                    tracked: false);

            return program == null
                ? null
                : _mapper.Map<ProgramResponse>(program);
        }

        public async Task<(bool Succeeded,
                   IEnumerable<string> Errors,
                   ProgramResponse? Data)> CreateProgramAsync(CreateProgramRequest request)
        {
            var program = new Program
            {
                ProgramName = request.ProgramName,
                Description = request.Description,
                TargetAgeFrom = request.TargetAgeFrom,
                TargetAgeTo = request.TargetAgeTo,
                Language = request.Language,
                Status = request.Status,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.ProgramRepository.AddAsync(program);
            await _unitOfWork.SaveChangesAsync();

            return (
                true,
                Enumerable.Empty<string>(),
                _mapper.Map<ProgramResponse>(program)
            );
        }

        public async Task<(bool Succeeded,
                   bool NotFound,
                   IEnumerable<string> Errors,
                   ProgramResponse? Data)> UpdateProgramAsync(int id, UpdateProgramRequest request)
        {
            var program = await _unitOfWork.ProgramRepository
                .GetFirstOrDefaultAsync(
                    filter: p => p.Id == id && !p.IsDeleted);

            if (program == null)
            {
                return (
                    false,
                    true,
                    Enumerable.Empty<string>(),
                    null
                );
            }

            program.ProgramName = request.ProgramName;
            program.Description = request.Description;
            program.TargetAgeFrom = request.TargetAgeFrom;
            program.TargetAgeTo = request.TargetAgeTo;
            program.Language = request.Language;
            program.Status = request.Status;
            program.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync();

            return (
                true,
                false,
                Enumerable.Empty<string>(),
                _mapper.Map<ProgramResponse>(program)
            );
        }

        public async Task<(bool Succeeded,
                   bool NotFound,
                   IEnumerable<string> Errors)> DeleteProgramAsync(int id)
        {
            var program = await _unitOfWork.ProgramRepository
                .GetFirstOrDefaultAsync(
                    filter: p => p.Id == id && !p.IsDeleted);

            if (program == null)
            {
                return (
                    false,
                    true,
                    Enumerable.Empty<string>()
                );
            }

            program.IsDeleted = true;
            program.UpdatedAt = DateTime.UtcNow;
            program.DeletedAt = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync();

            return (
                true,
                false,
                Enumerable.Empty<string>()
            );
        }
    }
}
