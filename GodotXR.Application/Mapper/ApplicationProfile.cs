using AutoMapper;
using GodotXR.Application.DTOs.Request.Analyze;
using GodotXR.Application.DTOs.Request.ChildProfile;
using GodotXR.Application.DTOs.Response.Analyze;
using GodotXR.Application.DTOs.Response.ChildProfile;
using GodotXR.Application.DTOs.Response.Classroom;
using GodotXR.Application.DTOs.Response.Enrollment;
using GodotXR.Application.DTOs.Response.EventLog;
using GodotXR.Application.DTOs.Response.Exercise;
using GodotXR.Application.DTOs.Response.ExerciseQuestion;
using GodotXR.Application.DTOs.Response.ExerciseType;
using GodotXR.Application.DTOs.Response.Lesson;
using GodotXR.Application.DTOs.Response.Program;
using GodotXR.Application.DTOs.Response.PronunciationDetail;
using GodotXR.Application.DTOs.Response.Result;
using GodotXR.Application.DTOs.Response.SchoolYear;
using GodotXR.Application.DTOs.Response.Semester;
using GodotXR.Application.DTOs.Response.User;
using GodotXR.Domain.Entities;

namespace GodotXR.Application.Mapper
{
    public class ApplicationProfile : Profile
    {
        public ApplicationProfile()
        {
            CreateMap<User, UserResponse>();

            CreateMap<User, UserWithChildrenProfileResponse>();

            CreateMap<Program, ProgramResponse>()
               .ForMember(dest => dest.Lessons, opt => opt.MapFrom(src =>
                   src.Lessons.Where(l => !l.IsDeleted).ToList()));

            CreateMap<Lesson, LessonSummaryResponse>();

            CreateMap<Lesson, LessonResponse>();

            CreateMap<SchoolYear, SchoolYearResponse>()
                .ForMember(dest => dest.SemesterCount, opt => opt.MapFrom(src =>
                    src.Semesters.Count(s => !s.IsDeleted)));

            CreateMap<Semester, SemesterResponse>()
                .ForMember(dest => dest.TeacherName, opt => opt.MapFrom(src =>
                    src.Teacher != null ? src.Teacher.FullName : string.Empty))
                .ForMember(dest => dest.SchoolYearName, opt => opt.MapFrom(src =>
                    src.SchoolYear != null ? src.SchoolYear.YearName : string.Empty))
                .ForMember(dest => dest.SchoolYearStartDate, opt => opt.MapFrom(src =>
                    src.SchoolYear.StartDate))
                .ForMember(dest => dest.SchoolYearEndDate, opt => opt.MapFrom(src =>
                    src.SchoolYear.EndDate))
                .ForMember(dest => dest.SchoolYearStatus, opt => opt.MapFrom(src =>
                    src.SchoolYear.Status))
                .ForMember(dest => dest.ClassroomCount, opt => opt.MapFrom(src =>
                    src.Classrooms.Count(c => !c.IsDeleted)));

            CreateMap<Classroom, ClassroomResponse>()
                .ForMember(dest => dest.TeacherName, opt => opt.MapFrom(src =>
                    src.User != null ? src.User.FullName : string.Empty))
                .ForMember(dest => dest.TeacherSpecialty, opt => opt.MapFrom(src =>
                    src.User != null ? src.User.Specialty : string.Empty))
                .ForMember(dest => dest.ProgramName, opt => opt.MapFrom(src =>
                    src.Program != null ? src.Program.ProgramName : string.Empty))
                .ForMember(dest => dest.ProgramLanguage, opt => opt.MapFrom(src =>
                    src.Program != null ? src.Program.Language : string.Empty))
                .ForMember(dest => dest.TargetAgeFrom, opt => opt.MapFrom(src =>
                    src.Program != null ? src.Program.TargetAgeFrom : 0))
                .ForMember(dest => dest.TargetAgeTo, opt => opt.MapFrom(src =>
                    src.Program != null ? src.Program.TargetAgeTo : 0))
                .ForMember(dest => dest.SemesterName, opt => opt.MapFrom(src =>
                    src.Semester != null ? src.Semester.SemesterName : string.Empty))
                .ForMember(dest => dest.EnrollmentCount, opt => opt.MapFrom(src =>
                    src.Enrollments.Count(e => !e.IsDeleted)));

            CreateMap<Exercise, ExerciseResponse>()
                .ForMember(dest => dest.TeacherName, opt => opt.MapFrom(src =>
                    src.Teacher != null ? src.Teacher.FullName : string.Empty))
                .ForMember(dest => dest.LessonName, opt => opt.MapFrom(src =>
                    src.Lesson != null ? src.Lesson.LessonName : string.Empty))
                .ForMember(dest => dest.TypeName, opt => opt.MapFrom(src =>
                    src.ExerciseType != null ? src.ExerciseType.TypeName : string.Empty))
                .ForMember(dest => dest.QuestionCount, opt => opt.MapFrom(src =>
                    src.ExerciseQuestions.Count(q => !q.IsDeleted)));

            CreateMap<ExerciseQuestion, ExerciseQuestionResponse>()
                .ForMember(dest => dest.ExerciseName, opt => opt.MapFrom(src =>
                    src.Exercise != null ? src.Exercise.ExerciseName : string.Empty))
                .ForMember(dest => dest.TeacherName, opt => opt.MapFrom(src =>
                    src.Teacher != null ? src.Teacher.FullName : string.Empty));

            CreateMap<CreateChildProfileRequest, ChildProfile>();

            CreateMap<ChildProfile, ChildProfileResponse>();

            CreateMap<ExerciseType, ExerciseTypeResponse>();

            CreateMap<Enrollment, EnrollmentResponse>()
                .ForMember(dest => dest.ChildFullName, opt => opt.MapFrom(src => src.Child.FullName))
                .ForMember(dest => dest.ChildLearningLevel, opt => opt.MapFrom(src => src.Child.LearningLevel))
                .ForMember(dest => dest.ClassName, opt => opt.MapFrom(src => src.Classroom.ClassName));

            CreateMap<Result, ResultResponse>()
                .ForMember(d => d.PronunciationDetails, o => o.MapFrom(s => s.PronunciationDetails))
                .ForMember(d => d.EventLogs, o => o.MapFrom(s => s.EventLogs));

            CreateMap<PronunciationDetail, PronunciationDetailResponse>();
            
            CreateMap<EventLog, EventLogResponse>();

            CreateMap<Analyze, AnalyzeResponse>();

            CreateMap<CreateAnalyzeRequest, Analyze>();

            CreateMap<UpdateAnalyzeRequest, Analyze>()
                .ForAllMembers(opts =>
                    opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
