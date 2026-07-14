using GodotXR.Application.DTOs.Request.Role;
using GodotXR.Application.DTOs.Response;
using GodotXR.Application.DTOs.Response.Role;
using GodotXR.Domain.Entities;
using GodotXR.Domain.IUnitOfWork;

namespace GodotXR.Application.Services
{
    public class RoleService : IRoleService
    {
        private readonly IUnitOfWork _unitOfWork;

        public RoleService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PagedResponse<RoleResponse>> GetListRoleAsync(int pageNumber, int pageSize)
        {
            var paged = await _unitOfWork.RoleRepository.GetPagedAsync(
                pageNumber, 
                pageSize, 
                r => r.IsActive && !r.IsDeleted);

            return new PagedResponse<RoleResponse>
            {
                PageNumber = paged.PageNumber,
                PageSize = paged.PageSize,
                TotalCount = paged.TotalCount,
                TotalPages = paged.TotalPages,
                Items = paged.Items.Select(MapToResponse).ToList(),
            };
        }

        public async Task<RoleResponse?> GetRoleByIdAsync(int id)
        {
            var role = await _unitOfWork.RoleRepository.GetFirstOrDefaultAsync(
                filter: r => r.Id == id && !r.IsDeleted,
                tracked: false);

            return role == null ? null : MapToResponse(role);
        }

        public async Task<(bool Succeeded, IEnumerable<string> Errors, RoleResponse? Data)> CreateRoleAsync(CreateRoleRequest request)
        {
            var errors = new List<string>();

            var exists = await _unitOfWork.RoleRepository.ExistsAsync(
                r => r.RoleName == request.RoleName &&
                     !r.IsDeleted);

            if (exists)
                errors.Add($"Role '{request.RoleName}' already exists.");

            if (errors.Any())
                return (false, errors, null);

            var role = new Role
            {
                RoleName = request.RoleName,
                Description = request.Description,
                IsActive = true
            };

            await _unitOfWork.RoleRepository.AddAsync(role);
            await _unitOfWork.SaveChangesAsync();

            return (true, Enumerable.Empty<string>(), MapToResponse(role)
            );
        }

        public async Task<(bool Succeeded,bool NotFound, IEnumerable<string> Errors, RoleResponse? Data)> UpdateRoleAsync(int id, UpdateRoleRequest request)
        {
            var role = await _unitOfWork.RoleRepository.GetFirstOrDefaultAsync(
                filter: r => r.Id == id && !r.IsDeleted);

            if (role == null)
                return (
                    false,
                    true,
                    Enumerable.Empty<string>(),
                    null
                );

            var errors = new List<string>();

            if (request.RoleName.HasValue &&
                request.RoleName.Value != role.RoleName)
            {
                var exists = await _unitOfWork.RoleRepository.ExistsAsync(
                    r => r.RoleName == request.RoleName.Value &&
                         r.Id != id &&
                         !r.IsDeleted);

                if (exists)
                    errors.Add($"Role '{request.RoleName}' already exists.");
            }

            if (errors.Any())
                return (false, false, errors, null);

            if (request.RoleName.HasValue)
                role.RoleName = request.RoleName.Value;

            if (request.Description != null)
                role.Description = request.Description;

            if (request.IsActive.HasValue)
                role.IsActive = request.IsActive.Value;

            role.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.RoleRepository.Update(role);
            await _unitOfWork.SaveChangesAsync();

            return (true, false, Enumerable.Empty<string>(), MapToResponse(role));
        }

        public async Task<(bool Succeeded, bool NotFound, IEnumerable<string> Errors)> DeleteRoleAsync(int id)
        {
            var role = await _unitOfWork.RoleRepository.GetFirstOrDefaultAsync(
                r => r.Id == id && !r.IsDeleted);

            if (role == null)
                return (false, true, Enumerable.Empty<string>());

            role.IsDeleted = true;
            role.IsActive = false;
            role.DeletedAt = DateTime.UtcNow;
            role.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.RoleRepository.Update(role);
            await _unitOfWork.SaveChangesAsync();

            return (true, false, Enumerable.Empty<string>());
        }

        private static RoleResponse MapToResponse(Role role) => new()
        {
            Id = role.Id,
            RoleName = role.RoleName.ToString(),
            Description = role.Description,
            IsActive = role.IsActive,
            CreatedAt = role.CreatedAt,
            UpdatedAt = role.UpdatedAt
        };
    }
}