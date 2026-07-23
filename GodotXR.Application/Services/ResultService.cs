using AutoMapper;
using GodotXR.Application.DTOs.Request.Result;
using GodotXR.Application.DTOs.Response;
using GodotXR.Application.DTOs.Response.Result;
using GodotXR.Domain.Entities;
using GodotXR.Domain.IUnitOfWork;
namespace GodotXR.Application.Services
{
    public class ResultService : IResultService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ResultService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<(bool Succeeded, bool NotFound, IEnumerable<string> Errors, ResultResponse? Data)> SubmitAsync(SubmitResultRequest request)
        {
            var errors = new List<string>();

            if (request.ExerciseId.HasValue && request.LessonId.HasValue)
                errors.Add("Chỉ được cung cấp một trong hai thông tin: mã bài tập (ExerciseId) hoặc mã bài học (LessonId).");

            if (!request.ExerciseId.HasValue && !request.LessonId.HasValue)
                errors.Add("Phải cung cấp ít nhất một thông tin: mã bài tập (ExerciseId) hoặc mã bài học (LessonId).");

            if (errors.Any())
                return (false, false, errors, null);

            var resultType = request.ExerciseId.HasValue
                ? ResultType.Exercise
                : ResultType.Lesson;

            // BR-80: idempotent — cùng SessionId không tạo duplicate
            var existing = await _unitOfWork.ResultRepository.GetBySessionIdAsync(request.SessionId);
            if (existing != null)
                return (true, false, Array.Empty<string>(), _mapper.Map<ResultResponse>(existing));

            // BR-75: Child phải active
            var child = await _unitOfWork.ChildProfileRepository.GetByIdAsync(request.ChildId);
            if (child == null)
                return (false, true, new[] { "Không tìm thấy hồ sơ trẻ." }, null);
            if (child.Status != "Active")
                errors.Add("Hồ sơ trẻ không ở trạng thái Hoạt động.");

            // BR-76: phải có active Enrollment
            var enrollments = await _unitOfWork.EnrollmentRepository.GetByChildIdAsync(request.ChildId);
            if (!enrollments.Any(e => e.Status == "Active" && !e.IsDeleted))
                errors.Add("Trẻ chưa có hồ sơ ghi danh hoạt động tại bất kỳ lớp nào.");

            // BR-84: Exercise phải tồn tại
            if (resultType == ResultType.Exercise)
            {
                var exercise = await _unitOfWork.ExerciseRepository.GetByIdAsync(request.ExerciseId!.Value);

                if (exercise == null)
                    return (false, true, new[] { "Không tìm thấy bài tập." }, null);

                // BR-74: Exercise phải active
                if (exercise.Status != "Active")
                    errors.Add("Bài tập không ở trạng thái Hoạt động.");
            }
            else
            {
                var lesson = await _unitOfWork.LessonRepository.GetByIdAsync(request.LessonId!.Value);

                if (lesson == null)
                    return (false, true, new[] { "Không tìm thấy bài học." }, null);

                if (lesson.Status != "Active")
                    errors.Add("Bài học không ở trạng thái Hoạt động.");
            }

            // BR-86: Score range
            if (request.Score < 0 || request.Score > 100)
                errors.Add("Điểm số phải nằm trong khoảng từ 0 đến 100.");

            // BR-87: Duration không âm
            if (request.DurationSeconds < 0)
                errors.Add("Thời lượng không được là số âm.");

            // BR-85: CompletionStatus hợp lệ
            var validStatuses = new[] { "Completed", "Incomplete" };
            if (!validStatuses.Contains(request.CompletionStatus))
                errors.Add("Trạng thái hoàn thành không hợp lệ. Chỉ chấp nhận: 'Completed' hoặc 'Incomplete'.");

            // BR-97: AccuracyScore range
            //foreach (var pd in request.PronunciationDetails)
            //{
            //    if (pd.AccuracyScore < 0 || pd.AccuracyScore > 100)
            //        errors.Add("AccuracyScore must be between 0 and 100.");
            //}

            if (errors.Any())
                return (false, false, errors, null);

            // BR-78: AttemptNumber tự tăng
            var attemptCount = await _unitOfWork.ResultRepository.GetAttemptCountAsync(request.ChildId, request.ExerciseId, request.LessonId);

            var result = new Result
            {
                SessionId = request.SessionId,

                ChildId = request.ChildId,

                ResultType = resultType,

                LessonId = request.LessonId,
                ExerciseId = request.ExerciseId,

                AttemptNumber = attemptCount + 1,

                CompletionStatus = request.CompletionStatus,
                Score = request.Score,
                ErrorCount = request.ErrorCount,
                CorrectCount = request.CorrectCount,

                StartedAt = request.StartedAt,
                CompletedAt = request.CompletedAt,

                DurationSeconds = request.DurationSeconds,

                InteractionLog = request.InteractionLog,
                FeedbackText = request.FeedbackText,

                IsFinalized = request.CompletionStatus == "Completed", // BR-89
            };

            await _unitOfWork.ResultRepository.AddAsync(result);
            await _unitOfWork.SaveChangesAsync(); // Save trước để có ResultId cho FK

            // BR-95, 98: PronunciationDetails phải có ResultId hợp lệ
            //foreach (var pdReq in request.PronunciationDetails)
            //{
            //    var pd = new PronunciationDetail
            //    {
            //        ResultId = result.Id,
            //        ExpectedPhoneme = pdReq.ExpectedPhoneme,
            //        ActualPhoneme = pdReq.ActualPhoneme,
            //        AccuracyScore = pdReq.AccuracyScore,
            //        IssueType = pdReq.IssueType,
            //        ReplayDataUrl = pdReq.ReplayDataUrl,
            //    };
            //    await _unitOfWork.PronunciationDetailRepository.AddAsync(pd);
            //}

            // BR-175: EventLogs
            //foreach (var evReq in request.EventLogs)
            //{
            //    var ev = new EventLog
            //    {
            //        ResultId = result.Id,
            //        ChildId = request.ChildId,
            //        EventType = evReq.EventType,
            //        EventTime = evReq.EventTime,
            //        Description = evReq.Description,
            //    };
            //    await _unitOfWork.EventLogRepository.AddAsync(ev);
            //}

            await _unitOfWork.SaveChangesAsync();
            var fullResult = await _unitOfWork.ResultRepository.GetWithDetailsAsync(result.Id);

            return (true, false, Array.Empty<string>(), _mapper.Map<ResultResponse>(fullResult));
        }

        public async Task<ApiResponse<ResultResponse>> GetByIdAsync(int id)
        {
            var result = await _unitOfWork.ResultRepository.GetWithDetailsAsync(id);
            if (result == null)
                return new ApiResponse<ResultResponse> { Success = false, Message = "Result not found." };

            return new ApiResponse<ResultResponse>
            {
                Success = true,
                Message = "Success.",
                Data = _mapper.Map<ResultResponse>(result)
            };
        }

        public async Task<ApiResponse<IEnumerable<ResultResponse>>> GetByChildIdAsync(int childId)
        {
            var child = await _unitOfWork.ChildProfileRepository.GetByIdAsync(childId);
            if (child == null)
                return new ApiResponse<IEnumerable<ResultResponse>> { Success = false, Message = "Child not found." };

            var results = await _unitOfWork.ResultRepository.GetByChildIdAsync(childId);
            return new ApiResponse<IEnumerable<ResultResponse>>
            {
                Success = true,
                Message = "Success.",
                Data = _mapper.Map<IEnumerable<ResultResponse>>(results)
            };
        }

        public async Task<ApiResponse<IEnumerable<ResultResponse>>> GetByExerciseIdAsync(int exerciseId)
        {
            var exercise = await _unitOfWork.ExerciseRepository.GetByIdAsync(exerciseId);
            if (exercise == null)
                return new ApiResponse<IEnumerable<ResultResponse>> { Success = false, Message = "Exercise not found." };

            var results = await _unitOfWork.ResultRepository.GetByExerciseIdAsync(exerciseId);
            return new ApiResponse<IEnumerable<ResultResponse>>
            {
                Success = true,
                Message = "Success.",
                Data = _mapper.Map<IEnumerable<ResultResponse>>(results)
            };
        }

        public async Task<ApiResponse<ResultResponse>> UpdateFeedbackAsync(int id, string feedbackText)
        {
            var result = await _unitOfWork.ResultRepository.GetByIdAsync(id);
            if (result == null)
                return new ApiResponse<ResultResponse> { Success = false, Message = "Result not found." };

            result.FeedbackText = feedbackText;
            _unitOfWork.ResultRepository.Update(result);
            await _unitOfWork.SaveChangesAsync();

            var fullResult = await _unitOfWork.ResultRepository.GetWithDetailsAsync(id);
            return new ApiResponse<ResultResponse>
            {
                Success = true,
                Message = "Feedback updated successfully.",
                Data = _mapper.Map<ResultResponse>(fullResult)
            };
        }
    }
}