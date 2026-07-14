using AutoMapper;
using GodotXR.Application.DTOs.Request.Enrollment;
using GodotXR.Application.DTOs.Response;
using GodotXR.Application.DTOs.Response.Enrollment;
using GodotXR.Domain.Entities;
using GodotXR.Domain.IUnitOfWork;

namespace GodotXR.Application.Services
{
    public class EnrollmentService : IEnrollmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public EnrollmentService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedResponse<EnrollmentResponse>> GetListEnrollmentAsync(
            int pageNumber, int pageSize,
            string? status = null,
            int? classId = null,
            string? learningLevel = null)
        {
            var all = (await _unitOfWork.EnrollmentRepository.GetAllWithDetailsAsync()).ToList();

            if (!string.IsNullOrWhiteSpace(status))
                all = all.Where(e => e.Status == status).ToList();
            if (classId.HasValue)
                all = all.Where(e => e.ClassId == classId.Value).ToList();
            if (!string.IsNullOrWhiteSpace(learningLevel))
                all = all.Where(e => e.Child.LearningLevel == learningLevel).ToList();

            var total = all.Count;
            var totalPages = total == 0 ? 0 : (int)Math.Ceiling(total / (double)pageSize);
            var items = all.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            var mapped = _mapper.Map<List<EnrollmentResponse>>(items);

            return new PagedResponse<EnrollmentResponse>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = total,
                TotalPages = totalPages,
                Items = mapped
            };
        }

        public async Task<EnrollmentResponse?> GetEnrollmentByIdAsync(int id)
        {
            var entity = await _unitOfWork.EnrollmentRepository.GetByIdWithDetailsAsync(id);
            return entity is null ? null : _mapper.Map<EnrollmentResponse>(entity);
        }

        public async Task<(bool Succeeded, IEnumerable<string> Errors, EnrollmentResponse? Data)>
            CreateEnrollmentAsync(CreateEnrollmentRequest request, int requesterId, string requesterRole)
        {
            var errors = new List<string>();
            var validStatuses = new[] { "Active", "Pending", "Completed", "Cancelled" };

            if (!validStatuses.Contains(request.Status))
            {
                errors.Add("Trạng thái không hợp lệ.");
                return (false, errors, null);
            }

            var child = await _unitOfWork.Repository<ChildProfile>()
                .GetFirstOrDefaultAsync(c => c.Id == request.ChildId && !c.IsDeleted);
            if (child is null)
            {
                errors.Add("Hồ sơ trẻ không tồn tại.");
                return (false, errors, null);
            }

            var classroom = await _unitOfWork.Repository<Classroom>()
                .GetFirstOrDefaultAsync(c => c.Id == request.ClassId && !c.IsDeleted);
            if (classroom is null)
            {
                errors.Add("Lớp học không tồn tại.");
                return (false, errors, null);
            }
            if (classroom.Status != "Active")
            {
                errors.Add("Lớp học không đang hoạt động.");
                return (false, errors, null);
            }
            if (requesterRole == "Teacher" && classroom.UserId != requesterId)
            {
                errors.Add("Giáo viên chỉ có thể ghi danh vào lớp học của mình.");
                return (false, errors, null);
            }
            var duplicate = await _unitOfWork.EnrollmentRepository
                .HasActiveEnrollmentAsync(request.ChildId, request.ClassId);
            if (duplicate)
            {
                errors.Add("Trẻ đã có ghi danh Active trong lớp học này.");
                return (false, errors, null);
            }

            var entity = new Enrollment
            {
                ChildId = request.ChildId,
                ClassId = request.ClassId,
                EnrollmentDate = request.EnrollmentDate,
                Status = request.Status,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<Enrollment>().AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            var result = await _unitOfWork.EnrollmentRepository.GetByIdWithDetailsAsync(entity.Id);
            return (true, errors, _mapper.Map<EnrollmentResponse>(result));
        }

        public async Task<(bool Succeeded, bool NotFound, IEnumerable<string> Errors, EnrollmentResponse? Data)>
            UpdateEnrollmentAsync(int id, UpdateEnrollmentRequest request, int requesterId, string requesterRole)
        {
            var errors = new List<string>();

            var entity = await _unitOfWork.EnrollmentRepository.GetByIdWithDetailsAsync(id);
            if (entity is null) return (false, true, errors, null);

            var validStatuses = new[] { "Active", "Pending", "Completed", "Cancelled" };
            if (!validStatuses.Contains(request.Status))
            {
                errors.Add("Trạng thái không hợp lệ.");
                return (false, false, errors, null);
            }

            var classroom = await _unitOfWork.Repository<Classroom>()
                .GetFirstOrDefaultAsync(c => c.Id == request.ClassId && !c.IsDeleted);
            if (classroom is null)
            {
                errors.Add("Lớp học không tồn tại.");
                return (false, false, errors, null);
            }
            if (requesterRole == "Teacher" && classroom.UserId != requesterId)
            {
                errors.Add("Giáo viên chỉ có thể cập nhật ghi danh trong lớp học của mình.");
                return (false, false, errors, null);
            }
            if (entity.ClassId != request.ClassId || entity.ChildId != request.ChildId)
            {
                var duplicate = await _unitOfWork.EnrollmentRepository
                    .HasActiveEnrollmentAsync(request.ChildId, request.ClassId, excludeId: id);
                if (duplicate)
                {
                    errors.Add("Trẻ đã có ghi danh Active trong lớp học này.");
                    return (false, false, errors, null);
                }
            }

            entity.ChildId = request.ChildId;
            entity.ClassId = request.ClassId;
            entity.EnrollmentDate = request.EnrollmentDate;
            entity.Status = request.Status;
            entity.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Repository<Enrollment>().Update(entity);
            await _unitOfWork.SaveChangesAsync();

            var result = await _unitOfWork.EnrollmentRepository.GetByIdWithDetailsAsync(entity.Id);
            return (true, false, errors, _mapper.Map<EnrollmentResponse>(result));
        }

        public async Task<(bool Succeeded, bool NotFound, IEnumerable<string> Errors, EnrollmentResponse? Data)>
            TransferEnrollmentAsync(int id, TransferEnrollmentRequest request, int requesterId, string requesterRole)
        {
            var errors = new List<string>();

            var entity = await _unitOfWork.EnrollmentRepository.GetByIdWithDetailsAsync(id);
            if (entity is null) return (false, true, errors, null);

            if (entity.Status != "Active")
            {
                errors.Add("Chỉ có thể chuyển lớp cho ghi danh đang Active.");
                return (false, false, errors, null);
            }

            var newClassroom = await _unitOfWork.Repository<Classroom>()
                .GetFirstOrDefaultAsync(c => c.Id == request.NewClassId && !c.IsDeleted);
            if (newClassroom is null)
            {
                errors.Add("Lớp học mới không tồn tại.");
                return (false, false, errors, null);
            }
            if (newClassroom.Status != "Active")
            {
                errors.Add("Lớp học mới không đang hoạt động.");
                return (false, false, errors, null);
            }
            if (requesterRole == "Teacher" && newClassroom.UserId != requesterId)
            {
                errors.Add("Giáo viên chỉ có thể chuyển sang lớp học của mình.");
                return (false, false, errors, null);
            }
            var duplicate = await _unitOfWork.EnrollmentRepository
                .HasActiveEnrollmentAsync(entity.ChildId, request.NewClassId, excludeId: id);
            if (duplicate)
            {
                errors.Add("Trẻ đã có ghi danh Active trong lớp học mới này.");
                return (false, false, errors, null);
            }

            entity.ClassId = request.NewClassId;
            entity.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Repository<Enrollment>().Update(entity);
            await _unitOfWork.SaveChangesAsync();

            var result = await _unitOfWork.EnrollmentRepository.GetByIdWithDetailsAsync(entity.Id);
            return (true, false, errors, _mapper.Map<EnrollmentResponse>(result));
        }

        public async Task<(bool Succeeded, bool NotFound, IEnumerable<string> Errors, EnrollmentResponse? Data)>
            ApproveEnrollmentAsync(int id, int requesterId, string requesterRole)
        {
            var errors = new List<string>();

            var entity = await _unitOfWork.EnrollmentRepository.GetByIdWithDetailsAsync(id);
            if (entity is null) return (false, true, errors, null);

            if (entity.Status != "Pending")
            {
                errors.Add("Chỉ có thể duyệt ghi danh đang ở trạng thái Chờ duyệt.");
                return (false, false, errors, null);
            }
            if (requesterRole == "Teacher" && entity.Classroom.UserId != requesterId)
            {
                errors.Add("Giáo viên chỉ có thể duyệt ghi danh trong lớp học của mình.");
                return (false, false, errors, null);
            }

            entity.Status = "Active";
            entity.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Repository<Enrollment>().Update(entity);
            await _unitOfWork.SaveChangesAsync();

            var result = await _unitOfWork.EnrollmentRepository.GetByIdWithDetailsAsync(entity.Id);
            return (true, false, errors, _mapper.Map<EnrollmentResponse>(result));
        }

        public async Task<(bool Succeeded, bool NotFound, IEnumerable<string> Errors)>
            DeleteEnrollmentAsync(int id)
        {
            var errors = new List<string>();

            var entity = await _unitOfWork.EnrollmentRepository.GetByIdWithDetailsAsync(id);
            if (entity is null) return (false, true, errors);
            entity.IsDeleted = true;
            entity.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.Repository<Enrollment>().Update(entity);
            await _unitOfWork.SaveChangesAsync();
            return (true, false, errors);
        }
        public async Task<IEnumerable<EnrollmentResponse>> GetEnrollmentsByChildIdAsync(int childId)
        {
            var all = (await _unitOfWork.EnrollmentRepository.GetAllWithDetailsAsync())
                .Where(e => e.ChildId == childId && !e.IsDeleted)
                .ToList();

            return _mapper.Map<List<EnrollmentResponse>>(all);
        }
    }
}