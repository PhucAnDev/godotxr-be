using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodotXR.Application.DTOs.Response.ExerciseQuestion
{
    public class ExerciseQuestionResponse
    {
        public int Id { get; set; }
        public int ExerciseId { get; set; }
        public string ExerciseName { get; set; } = string.Empty;
        public int TeacherId { get; set; }
        public string TeacherName { get; set; } = string.Empty;
        public string? Instruction { get; set; }
        public string QuestionSentence { get; set; } = string.Empty;
        public string AnswerSentence { get; set; } = string.Empty;
        public string InputType { get; set; } = string.Empty;
        public string? AudioURL { get; set; }
        public string? ImageURL { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
