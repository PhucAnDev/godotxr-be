using GodotXR.Application.DTOs.Request.Program;
using GodotXR.Application.DTOs.Response;
using GodotXR.Application.DTOs.Response.Program;

namespace GodotXR.Application.Services
{
    public interface IProgramService
    {
        Task<PagedResponse<ProgramResponse>> GetListProgramAsync(int pageNumber, int pageSize);

        Task<ProgramResponse?> GetProgramByIdAsync(int id);

        Task<(bool Succeeded, IEnumerable<string> Errors, ProgramResponse? Data)>
            CreateProgramAsync(CreateProgramRequest request);

        Task<(bool Succeeded, bool NotFound, IEnumerable<string> Errors, ProgramResponse? Data)>
            UpdateProgramAsync(int id, UpdateProgramRequest request);

        Task<(bool Succeeded, bool NotFound, IEnumerable<string> Errors)>
            DeleteProgramAsync(int id);
    }
}