using AutoMapper;
using GodotXR.Application.DTOs.Request.Exercise;
using GodotXR.Application.DTOs.Response;
using GodotXR.Application.DTOs.Response.Exercise;
using GodotXR.Domain.Entities;
using GodotXR.Domain.Enums;
using GodotXR.Domain.IUnitOfWork;
using GodotXR.Domain.Shared;

namespace GodotXR.Application.Services
{
    public class ExerciseService : IExerciseService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ExerciseService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedResponse<ExerciseResponse>> GetListAsync(
            int pageNumber, int pageSize,
            int? lessonId = null, int? typeId = null,
            int? teacherId = null, string? status = null)
        {
            var paged = await _unitOfWork.ExerciseRepository.GetPagedAsync(
                pageNumber, pageSize,
                predicate: e =>
                    !e.IsDeleted &&
                    (!lessonId.HasValue || e.LessonId == lessonId.Value) &&
                    (!typeId.HasValue || e.TypeId == typeId.Value) &&
                    (!teacherId.HasValue || e.TeacherId == teacherId.Value) &&
                    (string.IsNullOrWhiteSpace(status) || e.Status == status),
                orderBy: q => q.OrderByDescending(e => e.CreatedAt),
                includeProperties: "Teacher,Lesson,Lesson.Program,ExerciseType,ExerciseQuestions");

            return new PagedResponse<ExerciseResponse>
            {
                PageNumber = paged.PageNumber,
                PageSize = paged.PageSize,
                TotalCount = paged.TotalCount,
                TotalPages = paged.TotalPages,
                Items = _mapper.Map<List<ExerciseResponse>>(paged.Items)
            };
        }

        public async Task<ExerciseResponse?> GetByIdAsync(int id)
        {
            var entity = await _unitOfWork.ExerciseRepository
                .GetFirstOrDefaultAsync(
                    filter: e => e.Id == id && !e.IsDeleted,
                    includeProperties: "Teacher,Lesson,Lesson.Program,ExerciseType,ExerciseQuestions",
                    tracked: false);
            return entity == null ? null : _mapper.Map<ExerciseResponse>(entity);
        }

        public async Task<(bool Succeeded, IEnumerable<string> Errors, ExerciseResponse? Data)>
            CreateAsync(CreateExerciseRequest request)
        {
            // BR-67: DurationLimit > 0
            if (request.DurationLimit <= 0)
                return (false, new[] { "Giới hạn thời gian phải lớn hơn 0." }, null);

            // BR-68: DifficultyLevel hợp lệ
            if (!ExerciseConstants.AllowedDifficultyLevels.Contains(request.DifficultyLevel))
                return (false,
                    new[] { $"Mức độ khó phải thuộc một trong các giá trị: {string.Join(", ", ExerciseConstants.AllowedDifficultyLevels)}." },
                    null);

            // BR-70: Không tạo thẳng Status=Active vì chưa có Question nào
            if (request.Status == "Active")
                return (false,
                    new[] { "Không thể trực tiếp kiến tạo bài tập có trạng thái Hoạt động. Vui lòng tạo dưới dạng Chưa kích hoạt (Inactive) trước, thêm câu hỏi, sau đó mới kích hoạt bài tập." },
                    null);

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

            // BR-62/63: Lesson phải Active
            var lesson = await _unitOfWork.LessonRepository
                .GetFirstOrDefaultAsync(
                    filter: l => l.Id == request.LessonId
                              && !l.IsDeleted
                              && l.Status == "Active",
                    includeProperties: "Program",
                    tracked: false);
            if (lesson == null)
                return (false, new[] { "Không tìm thấy bài học hoặc bài học đã ngừng hoạt động." }, null);

            // BR-63: Program của Lesson phải Active
            if (lesson.Program == null || lesson.Program.IsDeleted || lesson.Program.Status != "Active")
                return (false, new[] { "Chương trình học chứa bài học này không ở trạng thái Hoạt động." }, null);

            // BR-66: ExerciseType phải Active
            var exerciseType = await _unitOfWork.ExerciseTypeRepository
                .GetFirstOrDefaultAsync(
                    filter: t => t.Id == request.TypeId
                              && !t.IsDeleted
                              && t.IsActive,
                    tracked: false);
            if (exerciseType == null)
                return (false, new[] { "Không tìm thấy loại bài tập hoặc loại bài tập đã bị ngừng hoạt động." }, null);

            var entity = new Exercise
            {
                TeacherId = request.TeacherId,
                LessonId = request.LessonId,
                TypeId = request.TypeId,
                ExerciseName = request.ExerciseName.Trim(),
                Instruction = request.Instruction,
                DifficultyLevel = request.DifficultyLevel,
                TargetSkill = request.TargetSkill,
                Language = request.Language,
                DurationLimit = request.DurationLimit,
                Status = request.Status,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.ExerciseRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            // Query lại để có đầy đủ navigation properties cho response
            var created = await _unitOfWork.ExerciseRepository
                .GetFirstOrDefaultAsync(
                    filter: e => e.Id == entity.Id,
                    includeProperties: "Teacher,Lesson,Lesson.Program,ExerciseType,ExerciseQuestions",
                    tracked: false);

            return (true, Enumerable.Empty<string>(), _mapper.Map<ExerciseResponse>(created));
        }

        public async Task<(bool Succeeded, bool NotFound, IEnumerable<string> Errors, ExerciseResponse? Data)>
            UpdateAsync(int id, UpdateExerciseRequest request)
        {
            // Load kèm ExerciseQuestions để check BR-70
            var entity = await _unitOfWork.ExerciseRepository
                .GetFirstOrDefaultAsync(
                    filter: e => e.Id == id && !e.IsDeleted,
                    includeProperties: "ExerciseQuestions");
            if (entity == null)
                return (false, true, Enumerable.Empty<string>(), null);

            // BR-67
            if (request.DurationLimit <= 0)
                return (false, false,
                    new[] { "Giới hạn thời gian phải lớn hơn 0." }, null);

            // BR-68
            if (!ExerciseConstants.AllowedDifficultyLevels.Contains(request.DifficultyLevel))
                return (false, false,
                    new[] { $"Mức độ khó phải thuộc một trong các giá trị: {string.Join(", ", ExerciseConstants.AllowedDifficultyLevels)}." },
                    null);

            // BR-70: Chỉ được activate nếu có ít nhất 1 Question
            if (request.Status == "Active")
            {
                var hasValidQuestions = entity.ExerciseQuestions.Any(q => !q.IsDeleted);
                if (!hasValidQuestions)
                    return (false, false,
                        new[] { "Không thể kích hoạt bài tập chưa có câu hỏi nào." }, null);
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

            // BR-62/63: Lesson phải Active
            var lesson = await _unitOfWork.LessonRepository
                .GetFirstOrDefaultAsync(
                    filter: l => l.Id == request.LessonId
                              && !l.IsDeleted
                              && l.Status == "Active",
                    includeProperties: "Program",
                    tracked: false);
            if (lesson == null)
                return (false, false, new[] { "Không tìm thấy bài học hoặc bài học đã ngừng hoạt động." }, null);

            // BR-63: Program phải Active
            if (lesson.Program == null || lesson.Program.IsDeleted || lesson.Program.Status != "Active")
                return (false, false, new[] { "Chương trình học chứa bài học này không ở trạng thái Hoạt động." }, null);

            // BR-66
            var exerciseType = await _unitOfWork.ExerciseTypeRepository
                .GetFirstOrDefaultAsync(
                    filter: t => t.Id == request.TypeId
                              && !t.IsDeleted
                              && t.IsActive,
                    tracked: false);
            if (exerciseType == null)
                return (false, false, new[] { "Không tìm thấy loại bài tập hoặc loại bài tập đã bị ngừng hoạt động." }, null);

            entity.TeacherId = request.TeacherId;
            entity.LessonId = request.LessonId;
            entity.TypeId = request.TypeId;
            entity.ExerciseName = request.ExerciseName.Trim();
            entity.Instruction = request.Instruction;
            entity.DifficultyLevel = request.DifficultyLevel;
            entity.TargetSkill = request.TargetSkill;
            entity.Language = request.Language;
            entity.DurationLimit = request.DurationLimit;
            entity.Status = request.Status;
            entity.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync();

            var updated = await _unitOfWork.ExerciseRepository
                .GetFirstOrDefaultAsync(
                    filter: e => e.Id == id,
                    includeProperties: "Teacher,Lesson,Lesson.Program,ExerciseType,ExerciseQuestions",
                    tracked: false);

            return (true, false, Enumerable.Empty<string>(), _mapper.Map<ExerciseResponse>(updated));
        }

        public async Task<(bool Succeeded, bool NotFound, IEnumerable<string> Errors)>
            DeleteAsync(int id)
        {
            var entity = await _unitOfWork.ExerciseRepository
                .GetFirstOrDefaultAsync(
                    filter: e => e.Id == id && !e.IsDeleted,
                    includeProperties: "Results");
            if (entity == null)
                return (false, true, Enumerable.Empty<string>());

            // BR-72: Không xóa nếu có historical Results
            if (entity.Results.Any())
                return (false, false,
                    new[] { "Không thể xóa bài tập đã có lịch sử kết quả làm bài của học sinh." });

            entity.IsDeleted = true;
            entity.DeletedAt = DateTime.UtcNow;
            entity.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync();
            return (true, false, Enumerable.Empty<string>());
        }
    }
}