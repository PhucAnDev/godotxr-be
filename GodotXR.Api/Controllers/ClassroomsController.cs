using GodotXR.Api.Contracts;
using GodotXR.Application.DTOs.Request.Classroom;
using GodotXR.Application.DTOs.Response;
using GodotXR.Application.DTOs.Response.Classroom;
using GodotXR.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GodotXR.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ClassroomsController : ControllerBase
    {
        private readonly IClassroomService _classroomService;

        public ClassroomsController(IClassroomService classroomService)
        {
            _classroomService = classroomService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Teacher,Parent")]
        [ProducesResponseType(typeof(ApiResponse<PagedResponse<ClassroomResponse>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get([FromQuery] PaginationQuery query)
        {
            var data = await _classroomService.GetListClassroomAsync(query.PageNumber, query.PageSize);

            return Ok(new ApiResponse<PagedResponse<ClassroomResponse>>
            {
                Success = true,
                Message = "OK",
                Data = data
            });
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = "Admin,Teacher,Parent")]
        [ProducesResponseType(typeof(ApiResponse<ClassroomResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            if (id <= 0)
                return BadRequest(new ApiResponse<ClassroomResponse>
                {
                    Success = false,
                    Message = "Mã định danh lớp học không hợp lệ."
                });

            var data = await _classroomService.GetClassroomByIdAsync(id);

            if (data == null)
                return NotFound(new ApiResponse<ClassroomResponse>
                {
                    Success = false,
                    Message = "Không tìm thấy lớp học."
                });

            return Ok(new ApiResponse<ClassroomResponse>
            {
                Success = true,
                Message = "OK",
                Data = data
            });
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<ClassroomResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateClassroomRequest request)
        {
            var (ok, errors, data) = await _classroomService.CreateClassroomAsync(request);

            if (!ok || data == null)
                return BadRequest(new ApiResponse<ClassroomResponse>
                {
                    Success = false,
                    Message = "Kiến tạo lớp học thất bại.",
                    Errors = errors.ToList()
                });

            return Ok(new ApiResponse<ClassroomResponse>
            {
                Success = true,
                Message = "Kiến tạo lớp học thành công.",
                Data = data
            });
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<ClassroomResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateClassroomRequest request)
        {
            if (id <= 0)
                return BadRequest(new ApiResponse<ClassroomResponse>
                {
                    Success = false,
                    Message = "Mã định danh lớp học không hợp lệ."
                });

            var (ok, notFound, errors, data) = await _classroomService.UpdateClassroomAsync(id, request);

            if (notFound)
                return NotFound(new ApiResponse<ClassroomResponse>
                {
                    Success = false,
                    Message = "Không tìm thấy lớp học."
                });

            if (!ok || data == null)
                return BadRequest(new ApiResponse<ClassroomResponse>
                {
                    Success = false,
                    Message = "Cập nhật thông tin lớp học thất bại.",
                    Errors = errors.ToList()
                });

            return Ok(new ApiResponse<ClassroomResponse>
            {
                Success = true,
                Message = "Cập nhật thông tin lớp học thành công.",
                Data = data
            });
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
                return BadRequest(new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Mã định danh lớp học không hợp lệ."
                });

            var (ok, notFound, errors) = await _classroomService.DeleteClassroomAsync(id);

            if (notFound)
                return NotFound(new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Không tìm thấy lớp học.",
                    Data = false
                });

            if (!ok)
                return BadRequest(new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Xóa lớp học thất bại.",
                    Errors = errors.ToList(),
                    Data = false
                });

            return Ok(new ApiResponse<bool>
            {
                Success = true,
                Message = "Xóa lớp học thành công.",
                Data = true
            });
        }
        [HttpGet("{teacherId:int}/classrooms")]
        [Authorize(Roles = "Admin,Teacher")]
        [ProducesResponseType(typeof(ApiResponse<PagedResponse<ClassroomResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetByTeacher(int teacherId, [FromQuery] PaginationQuery query)
        {
            if (teacherId <= 0)
                return BadRequest(new ApiResponse<PagedResponse<ClassroomResponse>>
                {
                    Success = false,
                    Message = "Mã định danh giáo viên không hợp lệ."
                });

            var data = await _classroomService.GetClassroomsByTeacherIdAsync(teacherId, query.PageNumber, query.PageSize);

            return Ok(new ApiResponse<PagedResponse<ClassroomResponse>>
            {
                Success = true,
                Message = "OK",
                Data = data
            });
        }
    }
}