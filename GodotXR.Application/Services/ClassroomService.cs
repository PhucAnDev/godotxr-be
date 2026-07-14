using AutoMapper;
using GodotXR.Application.DTOs.Request.Classroom;
using GodotXR.Application.DTOs.Response;
using GodotXR.Application.DTOs.Response.Classroom;
using GodotXR.Domain.Entities;
using GodotXR.Domain.Enums;
using GodotXR.Domain.IUnitOfWork;

namespace GodotXR.Application.Services
{
    public class ClassroomService : IClassroomService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ClassroomService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedResponse<ClassroomResponse>> GetListClassroomAsync(int pageNumber, int pageSize)
        {
            var paged = await _unitOfWork.ClassroomRepository.GetPagedAsync(
                pageNumber, pageSize,
                predicate: c => !c.IsDeleted,
                orderBy: q => q.OrderByDescending(c => c.StartDate),
                includeProperties: "User,User.Role,Program,Semester,Enrollments"
            );

            return new PagedResponse<ClassroomResponse>
            {
                PageNumber = paged.PageNumber,
                PageSize = paged.PageSize,
                TotalCount = paged.TotalCount,
                TotalPages = paged.TotalPages,
                Items = _mapper.Map<List<ClassroomResponse>>(paged.Items)
            };
        }

        public async Task<ClassroomResponse?> GetClassroomByIdAsync(int id)
        {
            var classroom = await _unitOfWork.ClassroomRepository
                .GetFirstOrDefaultAsync(
                    filter: c => c.Id == id && !c.IsDeleted,
                    includeProperties: "User,User.Role,Program,Semester,Enrollments",
                    tracked: false);

            return classroom == null ? null : _mapper.Map<ClassroomResponse>(classroom);
        }

        public async Task<(bool Succeeded, IEnumerable<string> Errors, ClassroomResponse? Data)>
            CreateClassroomAsync(CreateClassroomRequest request)
        {
            if (request.StartDate >= request.EndDate)
                return (false, new[] { "Start date must be before end date." }, null);
            var teacher = await _unitOfWork.UserRepository
                .GetFirstOrDefaultAsync(
                    filter: u => u.Id == request.UserId
                              && !u.IsDeleted
                              && u.IsActive
                              && u.Role.RoleName == UserRole.Teacher,
                    includeProperties: "Role",
                    tracked: false);

            if (teacher == null)
                return (false, new[] { "Teacher not found, inactive, or does not have Teacher role." }, null);
            var program = await _unitOfWork.ProgramRepository
                .GetFirstOrDefaultAsync(
                    filter: p => p.Id == request.ProgramId
                              && !p.IsDeleted
                              && p.Status == "Active",
                    tracked: false);

            if (program == null)
                return (false, new[] { "Program not found or inactive." }, null);
            var semester = await _unitOfWork.SemesterRepository
                .GetFirstOrDefaultAsync(
                    filter: s => s.Id == request.SemesterId && !s.IsDeleted,
                    tracked: false);

            if (semester == null)
                return (false, new[] { "Semester not found." }, null);
            if (request.StartDate < semester.StartDate || request.EndDate > semester.EndDate)
                return (false, new[] { "Classroom period must fall within the semester period." }, null);

            var classroom = new Classroom
            {
                UserId = request.UserId,
                ProgramId = request.ProgramId,
                SemesterId = request.SemesterId,
                ClassName = request.ClassName.Trim(),
                Description = request.Description,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                Status = request.Status,
                CreatedAt = DateTime.UtcNow
            };
            await _unitOfWork.ClassroomRepository.AddAsync(classroom);
            await _unitOfWork.SaveChangesAsync();
            var created = await _unitOfWork.ClassroomRepository
                .GetFirstOrDefaultAsync(
                    filter: c => c.Id == classroom.Id,
                    includeProperties: "User,User.Role,Program,Semester,Enrollments",
                    tracked: false);

            return (true, Enumerable.Empty<string>(), _mapper.Map<ClassroomResponse>(created));
        }

        public async Task<(bool Succeeded, bool NotFound, IEnumerable<string> Errors, ClassroomResponse? Data)>
            UpdateClassroomAsync(int id, UpdateClassroomRequest request)
        {
            var classroom = await _unitOfWork.ClassroomRepository
                .GetFirstOrDefaultAsync(
                    filter: c => c.Id == id && !c.IsDeleted,
                    includeProperties: "Enrollments");

            if (classroom == null)
                return (false, true, Enumerable.Empty<string>(), null);
            if (request.StartDate >= request.EndDate)
                return (false, false, new[] { "Start date must be before end date." }, null);
            var teacher = await _unitOfWork.UserRepository
                .GetFirstOrDefaultAsync(
                    filter: u => u.Id == request.UserId
                              && !u.IsDeleted
                              && u.IsActive
                              && u.Role.RoleName == UserRole.Teacher,
                    includeProperties: "Role",
                    tracked: false);

            if (teacher == null)
                return (false, false, new[] { "Teacher not found, inactive, or does not have Teacher role." }, null);
            var program = await _unitOfWork.ProgramRepository
                .GetFirstOrDefaultAsync(
                    filter: p => p.Id == request.ProgramId
                              && !p.IsDeleted
                              && p.Status == "Active",
                    tracked: false);

            if (program == null)
                return (false, false, new[] { "Program not found or inactive." }, null);
            var semester = await _unitOfWork.SemesterRepository
                .GetFirstOrDefaultAsync(
                    filter: s => s.Id == request.SemesterId && !s.IsDeleted,
                    tracked: false);

            if (semester == null)
                return (false, false, new[] { "Semester not found." }, null);
            if (request.StartDate < semester.StartDate || request.EndDate > semester.EndDate)
                return (false, false, new[] { "Classroom period must fall within the semester period." }, null);

            classroom.UserId = request.UserId;
            classroom.ProgramId = request.ProgramId;
            classroom.SemesterId = request.SemesterId;
            classroom.ClassName = request.ClassName.Trim();
            classroom.Description = request.Description;
            classroom.StartDate = request.StartDate;
            classroom.EndDate = request.EndDate;
            classroom.Status = request.Status;
            classroom.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync();

            var updated = await _unitOfWork.ClassroomRepository
                .GetFirstOrDefaultAsync(
                    filter: c => c.Id == id,
                    includeProperties: "User,User.Role,Program,Semester,Enrollments",
                    tracked: false);

            return (true, false, Enumerable.Empty<string>(), _mapper.Map<ClassroomResponse>(updated));
        }

        public async Task<(bool Succeeded, bool NotFound, IEnumerable<string> Errors)>
            DeleteClassroomAsync(int id)
        {
            var classroom = await _unitOfWork.ClassroomRepository
                .GetFirstOrDefaultAsync(
                    filter: c => c.Id == id && !c.IsDeleted,
                    includeProperties: "Enrollments");

            if (classroom == null)
                return (false, true, Enumerable.Empty<string>());
            if (classroom.Enrollments.Any(e => !e.IsDeleted && e.Status == "Active"))
                return (false, false, new[] { "Cannot delete a classroom that has active enrollments." });

            if (classroom.Status == "Active")
                return (false, false, new[] { "Cannot delete an active classroom." });

            classroom.IsDeleted = true;
            classroom.DeletedAt = DateTime.UtcNow;
            classroom.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync();

            return (true, false, Enumerable.Empty<string>());
        }
        public async Task<PagedResponse<ClassroomResponse>> GetClassroomsByTeacherIdAsync(int teacherId, int pageNumber, int pageSize)
        {
            var teacher = await _unitOfWork.UserRepository
                .GetFirstOrDefaultAsync(
                    filter: u => u.Id == teacherId && !u.IsDeleted && u.IsActive && u.Role.RoleName == UserRole.Teacher,
                    includeProperties: "Role",
                    tracked: false);

            if (teacher == null)
                return new PagedResponse<ClassroomResponse>
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalCount = 0,
                    TotalPages = 0,
                    Items = new List<ClassroomResponse>()
                };

            var paged = await _unitOfWork.ClassroomRepository.GetPagedAsync(
                pageNumber, pageSize,
                predicate: c => !c.IsDeleted && c.UserId == teacherId,
                orderBy: q => q.OrderByDescending(c => c.StartDate),
                includeProperties: "User,User.Role,Program,Semester,Enrollments"
            );

            return new PagedResponse<ClassroomResponse>
            {
                PageNumber = paged.PageNumber,
                PageSize = paged.PageSize,
                TotalCount = paged.TotalCount,
                TotalPages = paged.TotalPages,
                Items = _mapper.Map<List<ClassroomResponse>>(paged.Items)
            };
        }
    }
}