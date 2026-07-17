using AutoMapper;
using GodotXR.Application.DTOs.Request.ExerciseQuestion;
using GodotXR.Application.DTOs.Response;
using GodotXR.Application.DTOs.Response.ExerciseQuestion;
using GodotXR.Domain.Entities;
using GodotXR.Domain.Enums;
using GodotXR.Domain.IUnitOfWork;
using GodotXR.Domain.Shared;

namespace GodotXR.Application.Services
{
    public class ExerciseQuestionService : IExerciseQuestionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ExerciseQuestionService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // BR-71: Parse extension từ URL an toàn
        private static string? GetExtensionFromUrl(string url)
        {
            try
            {
                var path = new Uri(url).AbsolutePath;
                return Path.GetExtension(path).ToLowerInvariant();
            }
            catch
            {
                return null;
            }
        }

        public async Task<PagedResponse<ExerciseQuestionResponse>> GetListAsync(
            int pageNumber, int pageSize,
            int? exerciseId = null, int? teacherId = null)
        {
            var paged = await _unitOfWork.ExerciseQuestionRepository.GetPagedAsync(
                pageNumber, pageSize,
                predicate: q =>
                    !q.IsDeleted &&
                    (!exerciseId.HasValue || q.ExerciseId == exerciseId.Value) &&
                    (!teacherId.HasValue || q.TeacherId == teacherId.Value),
                orderBy: q => q.OrderByDescending(x => x.CreatedAt),
                includeProperties: "Exercise,Teacher");

            return new PagedResponse<ExerciseQuestionResponse>
            {
                PageNumber = paged.PageNumber,
                PageSize = paged.PageSize,
                TotalCount = paged.TotalCount,
                TotalPages = paged.TotalPages,
                Items = _mapper.Map<List<ExerciseQuestionResponse>>(paged.Items)
            };
        }

        public async Task<ExerciseQuestionResponse?> GetByIdAsync(int id)
        {
            var entity = await _unitOfWork.ExerciseQuestionRepository
                .GetFirstOrDefaultAsync(
                    filter: q => q.Id == id && !q.IsDeleted,
                    includeProperties: "Exercise,Teacher",
                    tracked: false);
            return entity == null ? null : _mapper.Map<ExerciseQuestionResponse>(entity);
        }

        public async Task<(bool Succeeded, IEnumerable<string> Errors, ExerciseQuestionResponse? Data)>
            CreateAsync(CreateExerciseQuestionRequest request)
        {
            // BR-69: validate 3 required fields ở service layer
            if (string.IsNullOrWhiteSpace(request.QuestionSentence))
                return (false, new[] { "Vui lòng nhập câu hỏi mẫu." }, null);
            if (string.IsNullOrWhiteSpace(request.AnswerSentence))
                return (false, new[] { "Vui lòng nhập câu trả lời mẫu." }, null);
            if (string.IsNullOrWhiteSpace(request.InputType))
                return (false, new[] { "Vui lòng nhập loại câu hỏi." }, null);

            // BR-71: validate AudioURL extension
            if (!string.IsNullOrWhiteSpace(request.AudioURL))
            {
                var audioExt = GetExtensionFromUrl(request.AudioURL);
                if (audioExt == null || !ExerciseConstants.AllowedAudioExtensions.Contains(audioExt))
                    return (false,
                        new[] { $"Tập tin âm thanh phải thuộc loại: {string.Join(", ", ExerciseConstants.AllowedAudioExtensions)}." },
                        null);
            }

            // BR-71: validate ImageURL extension
            if (!string.IsNullOrWhiteSpace(request.ImageURL))
            {
                var imageExt = GetExtensionFromUrl(request.ImageURL);
                if (imageExt == null || !ExerciseConstants.AllowedImageExtensions.Contains(imageExt))
                    return (false,
                        new[] { $"Tập tin hình ảnh phải thuộc loại: {string.Join(", ", ExerciseConstants.AllowedImageExtensions)}." },
                        null);
            }

            // BR-56: Teacher hợp lệ
            var teacher = await _unitOfWork.UserRepository
                .GetFirstOrDefaultAsync(
                    filter: u => u.Id == request.TeacherId
                              && !u.IsDeleted
                              && u.IsActive
                              && u.Role.RoleName == UserRole.Teacher,
                    includeProperties: "Role",
                    tracked: false);
            if (teacher == null)
                return (false, new[] { "Không tìm thấy giáo viên phụ trách hoặc thông tin giáo viên không hợp lệ." }, null);

            // BR-60: Exercise phải tồn tại (không check Active để tránh deadlock với BR-70)
            var exercise = await _unitOfWork.ExerciseRepository
                .GetFirstOrDefaultAsync(
                    filter: e => e.Id == request.ExerciseId && !e.IsDeleted,
                    tracked: false);
            if (exercise == null)
                return (false, new[] { "Không tìm thấy bài tập." }, null);

            var entity = new ExerciseQuestion
            {
                ExerciseId = request.ExerciseId,
                TeacherId = request.TeacherId,
                Instruction = request.Instruction,
                QuestionSentence = request.QuestionSentence.Trim(),
                AnswerSentence = request.AnswerSentence.Trim(),
                InputType = request.InputType,
                AudioURL = request.AudioURL,
                ImageURL = request.ImageURL,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.ExerciseQuestionRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            var created = await _unitOfWork.ExerciseQuestionRepository
                .GetFirstOrDefaultAsync(
                    filter: q => q.Id == entity.Id,
                    includeProperties: "Exercise,Teacher",
                    tracked: false);

            return (true, Enumerable.Empty<string>(), _mapper.Map<ExerciseQuestionResponse>(created));
        }

        public async Task<(bool Succeeded, bool NotFound, IEnumerable<string> Errors, ExerciseQuestionResponse? Data)>
            UpdateAsync(int id, UpdateExerciseQuestionRequest request)
        {
            var entity = await _unitOfWork.ExerciseQuestionRepository
                .GetFirstOrDefaultAsync(filter: q => q.Id == id && !q.IsDeleted);
            if (entity == null)
                return (false, true, Enumerable.Empty<string>(), null);

            // BR-69
            if (string.IsNullOrWhiteSpace(request.QuestionSentence))
                return (false, false, new[] { "Vui lòng nhập câu hỏi mẫu." }, null);
            if (string.IsNullOrWhiteSpace(request.AnswerSentence))
                return (false, false, new[] { "Vui lòng nhập câu trả lời mẫu." }, null);
            if (string.IsNullOrWhiteSpace(request.InputType))
                return (false, false, new[] { "Vui lòng nhập loại câu hỏi." }, null);

            // BR-71: AudioURL
            if (!string.IsNullOrWhiteSpace(request.AudioURL))
            {
                var audioExt = GetExtensionFromUrl(request.AudioURL);
                if (audioExt == null || !ExerciseConstants.AllowedAudioExtensions.Contains(audioExt))
                    return (false, false,
                        new[] { $"Tập tin âm thanh phải thuộc loại: {string.Join(", ", ExerciseConstants.AllowedAudioExtensions)}." },
                        null);
            }

            // BR-71: ImageURL
            if (!string.IsNullOrWhiteSpace(request.ImageURL))
            {
                var imageExt = GetExtensionFromUrl(request.ImageURL);
                if (imageExt == null || !ExerciseConstants.AllowedImageExtensions.Contains(imageExt))
                    return (false, false,
                        new[] { $"Tập tin hình ảnh phải thuộc loại: {string.Join(", ", ExerciseConstants.AllowedImageExtensions)}." },
                        null);
            }

            // BR-56
            var teacher = await _unitOfWork.UserRepository
                .GetFirstOrDefaultAsync(
                    filter: u => u.Id == request.TeacherId
                              && !u.IsDeleted
                              && u.IsActive
                              && u.Role.RoleName == UserRole.Teacher,
                    includeProperties: "Role",
                    tracked: false);
            if (teacher == null)
                return (false, false, new[] { "Không tìm thấy giáo viên phụ trách hoặc thông tin giáo viên không hợp lệ." }, null);

            // BR-60: Exercise tồn tại
            var exercise = await _unitOfWork.ExerciseRepository
                .GetFirstOrDefaultAsync(
                    filter: e => e.Id == request.ExerciseId && !e.IsDeleted,
                    tracked: false);
            if (exercise == null)
                return (false, false, new[] { "Không tìm thấy bài tập." }, null);

            entity.ExerciseId = request.ExerciseId;
            entity.TeacherId = request.TeacherId;
            entity.Instruction = request.Instruction;
            entity.QuestionSentence = request.QuestionSentence.Trim();
            entity.AnswerSentence = request.AnswerSentence.Trim();
            entity.InputType = request.InputType;
            entity.AudioURL = request.AudioURL;
            entity.ImageURL = request.ImageURL;
            entity.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync();

            var updated = await _unitOfWork.ExerciseQuestionRepository
                .GetFirstOrDefaultAsync(
                    filter: q => q.Id == id,
                    includeProperties: "Exercise,Teacher",
                    tracked: false);

            return (true, false, Enumerable.Empty<string>(), _mapper.Map<ExerciseQuestionResponse>(updated));
        }

        public async Task<(bool Succeeded, bool NotFound, IEnumerable<string> Errors)>
            DeleteAsync(int id)
        {
            var entity = await _unitOfWork.ExerciseQuestionRepository
                .GetFirstOrDefaultAsync(
                    filter: q => q.Id == id && !q.IsDeleted,
                    includeProperties: "Exercise.Results",
                    tracked: false);
            if (entity == null)
                return (false, true, Enumerable.Empty<string>());

            // BR-72: Không xóa nếu Exercise đã có historical Results
            if (entity.Exercise?.Results != null && entity.Exercise.Results.Any())
                return (false, false,
                    new[] { "Cannot delete a question whose exercise has historical results." });

            entity.IsDeleted = true;
            entity.DeletedAt = DateTime.UtcNow;
            entity.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync();
            return (true, false, Enumerable.Empty<string>());
        }
    }
}