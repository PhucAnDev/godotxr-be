using System.ComponentModel.DataAnnotations;

namespace GodotXR.Domain.Entities
{
    public class ExerciseType : BaseEntity
    {
        [Required, MaxLength(100)]
        public string TypeName { get; set; } = string.Empty;

        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation Properties
        public ICollection<Exercise> Exercises { get; set; } = new List<Exercise>();
    }
}