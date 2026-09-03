using AutoMapper;
using GodotXR.Application.DTOs.Request.ChildSpeechAccuracy;
using GodotXR.Application.DTOs.Response.ChildSpeechAccuracy;
using GodotXR.Domain.Entities;
using GodotXR.Domain.IUnitOfWork;

namespace GodotXR.Application.Services
{
    public class ChildSpeechAccuracyService : IChildSpeechAccuracyService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ChildSpeechAccuracyService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ChildSpeechAccuracyResponse>> GetByChildIdAsync(int childId)
        {
            var list = await _unitOfWork.ChildSpeechAccuracyRepository.GetByChildIdAsync(childId);
            return _mapper.Map<IEnumerable<ChildSpeechAccuracyResponse>>(list);
        }

        public async Task<IEnumerable<ChildSpeechAccuracyResponse>> GetBySessionIdAsync(string sessionId)
        {
            var list = await _unitOfWork.ChildSpeechAccuracyRepository.GetBySessionIdAsync(sessionId);
            return _mapper.Map<IEnumerable<ChildSpeechAccuracyResponse>>(list);
        }

        public async Task<IEnumerable<ChildSpeechAccuracyResponse>> GetByLessonIdAsync(int lessonId)
        {
            var list = await _unitOfWork.ChildSpeechAccuracyRepository.GetByLessonIdAsync(lessonId);
            return _mapper.Map<IEnumerable<ChildSpeechAccuracyResponse>>(list);
        }

        public async Task<IEnumerable<ChildSpeechAccuracyResponse>> GetByChunkAsync(int childProfileId, string sessionId, int audioChunkIndex)
        {
            var list = await _unitOfWork.ChildSpeechAccuracyRepository.GetByChunkAsync(childProfileId, sessionId, audioChunkIndex);
            return _mapper.Map<IEnumerable<ChildSpeechAccuracyResponse>>(list);
        }

        public async Task<ChildSpeechAccuracyResponse> CreateAsync(CreateChildSpeechAccuracyRequest request)
        {
            var entity = _mapper.Map<ChildSpeechAccuracy>(request);
            await _unitOfWork.ChildSpeechAccuracyRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<ChildSpeechAccuracyResponse>(entity);
        }

        public async Task<int> CreateBatchAsync(IEnumerable<CreateChildSpeechAccuracyRequest> requests)
        {
            int count = 0;
            foreach (var req in requests)
            {
                var entity = _mapper.Map<ChildSpeechAccuracy>(req);
                await _unitOfWork.ChildSpeechAccuracyRepository.AddAsync(entity);
                count++;
            }
            if (count > 0)
            {
                await _unitOfWork.SaveChangesAsync();
            }
            return count;
        }
    }
}
