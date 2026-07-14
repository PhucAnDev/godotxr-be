using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodotXR.Application.DTOs.Response.Exercise
{
    public class ExerciseResponse
    {
        public int Id { get; set; }
        public int TeacherId { get; set; }
        public string TeacherName { get; set; } = string.Empty;
        public int LessonId { get; set; }
        public string LessonName { get; set; } = string.Empty;
        public int TypeId { get; set; }
        public string TypeName { get; set; } = string.Empty;
        public string ExerciseName { get; set; } = string.Empty;
        public string? Instruction { get; set; }
        public string DifficultyLevel { get; set; } = string.Empty;
        public string TargetSkill { get; set; } = string.Empty;
        public string Language { get; set; } = string.Empty;
        public int DurationLimit { get; set; }
        public string Status { get; set; } = string.Empty;
        public int QuestionCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
