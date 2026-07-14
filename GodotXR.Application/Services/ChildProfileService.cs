using AutoMapper;
using GodotXR.Application.DTOs.Request.ChildProfile;
using GodotXR.Application.DTOs.Response;
using GodotXR.Application.DTOs.Response.ChildProfile;
using GodotXR.Domain.IUnitOfWork;
using GodotXR.Domain.Entities;

namespace GodotXR.Application.Services
{
    public class ChildProfileService : IChildProfileService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ChildProfileService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedResponse<ChildProfileResponse>> GetListChildProfileAsync(int pageNumber, int pageSize)
        {
            var paged = await _unitOfWork.ChildProfileRepository.GetPagedAsync(
                pageNumber,
                pageSize,
                r => !r.IsDeleted);

            return new PagedResponse<ChildProfileResponse>
            {
                PageNumber = paged.PageNumber,
                PageSize = paged.PageSize,
                TotalCount = paged.TotalCount,
                TotalPages = paged.TotalPages,
                Items = _mapper.Map<List<ChildProfileResponse>>(paged.Items)
            };
        }

        public async Task<ChildProfileResponse?> GetChildProfileByIdAsync(int id)
        {
            var classroom = await _unitOfWork.ChildProfileRepository
                .GetFirstOrDefaultAsync(
                    filter: c => c.Id == id && !c.IsDeleted,
                    tracked: false);

            return classroom == null ? null : _mapper.Map<ChildProfileResponse>(classroom);
        }

        public async Task<(bool Succeeded, IEnumerable<string> Errors, ChildProfileResponse? Data)>
            CreateChildProfileAsync(CreateChildProfileRequest request)
        {
            var errors = new List<string>();

            var user = await _unitOfWork.UserRepository.GetFirstOrDefaultAsync(
                u => u.Id == request.UserId && !u.IsDeleted && u.IsActive,
                tracked: false);

            if (user == null)
            {
                errors.Add("User does not exist.");
                return (false, errors, null);
            }

            var childProfile = _mapper.Map<ChildProfile>(request);

            await _unitOfWork.ChildProfileRepository.AddAsync(childProfile);

            var result = await _unitOfWork.SaveChangesAsync();

            if (result <= 0)
            {
                errors.Add("Failed to create child profile.");
                return (false, errors, null);
            }

            var response = _mapper.Map<ChildProfileResponse>(childProfile);

            return (true, Enumerable.Empty<string>(), response);
        }

        public async Task<(bool Succeeded, bool NotFound, IEnumerable<string> Errors, ChildProfileResponse? Data)>
     UpdateChildProfileAsync(int id, UpdateChildProfileRequest request)
        {
            var errors = new List<string>();

            var childProfile = await _unitOfWork.ChildProfileRepository
                .GetFirstOrDefaultAsync(
                    c => c.Id == id && !c.IsDeleted);

            if (childProfile == null)
            {
                errors.Add("Child profile not found.");
                return (false, true, errors, null);
            }

            if (request.UserId.HasValue)
            {
                var user = await _unitOfWork.UserRepository.GetFirstOrDefaultAsync(
                    u => u.Id == request.UserId.Value
                         && !u.IsDeleted
                         && u.IsActive,
                    tracked: false);

                if (user == null)
                {
                    errors.Add("User does not exist.");
                    return (false, false, errors, null);
                }

                childProfile.UserId = request.UserId.Value;
            }

            if (!string.IsNullOrWhiteSpace(request.Avatar))
                childProfile.Avatar = request.Avatar;

            if (!string.IsNullOrWhiteSpace(request.FullName))
                childProfile.FullName = request.FullName;

            if (request.Age.HasValue)
                childProfile.Age = request.Age.Value;

            if (!string.IsNullOrWhiteSpace(request.Gender))
                childProfile.Gender = request.Gender;

            if (!string.IsNullOrWhiteSpace(request.LearningLevel))
                childProfile.LearningLevel = request.LearningLevel;

            if (request.Note != null)
                childProfile.Note = request.Note;

            if (!string.IsNullOrWhiteSpace(request.Status))
                childProfile.Status = request.Status;

            _unitOfWork.ChildProfileRepository.Update(childProfile);

            var result = await _unitOfWork.SaveChangesAsync();

            if (result <= 0)
            {
                errors.Add("Failed to update child profile.");
                return (false, false, errors, null);
            }

            var response = _mapper.Map<ChildProfileResponse>(childProfile);

            return (true, false, Enumerable.Empty<string>(), response);
        }

        public async Task<(bool Succeeded, bool NotFound, IEnumerable<string> Errors)> DeleteChildProfileAsync(int id)
        {
            var childProfile = await _unitOfWork.ChildProfileRepository
                .GetFirstOrDefaultAsync(
                    c => c.Id == id && !c.IsDeleted);

            if (childProfile == null)
            {
                return (
                    Succeeded: false,
                    NotFound: true,
                    Errors: ["Child profile not found."]
                );
            }

            childProfile.IsDeleted = true;
            childProfile.DeletedAt = DateTime.UtcNow;
            childProfile.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.ChildProfileRepository.Update(childProfile);

            var result = await _unitOfWork.SaveChangesAsync();

            if (result <= 0)
            {
                return (
                    Succeeded: false,
                    NotFound: false,
                    Errors: ["Failed to delete child profile."]
                );
            }

            return (
                Succeeded: true,
                NotFound: false,
                Errors: Enumerable.Empty<string>()
            );
        }

        public async Task<IEnumerable<ChildProfileResponse>> GetChildProfilesByParentIdAsync(int parentId)
        {
            var childProfiles = await _unitOfWork.ChildProfileRepository
                .GetAllAsync(
                    filter: c => c.UserId == parentId && !c.IsDeleted );

            return _mapper.Map<IEnumerable<ChildProfileResponse>>(childProfiles);
        }
    }
}