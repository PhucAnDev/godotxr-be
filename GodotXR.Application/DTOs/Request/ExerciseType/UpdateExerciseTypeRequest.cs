using System.ComponentModel.DataAnnotations;
namespace GodotXR.Application.DTOs.Request.ExerciseType
{
    public class UpdateExerciseTypeRequest
    {
        [Required, MaxLength(100)]
        public string TypeName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
    }
}