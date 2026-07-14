using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace GodotXR.Domain.Entities
{
    public class User : BaseEntity
    {
        [MaxLength(500)]
        public string? Avatar { get; set; }
        [MaxLength(255)]
        public string PasswordHash { get; set; } = string.Empty;
        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;
        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;
        [Phone]
        [MaxLength(20)]
        public string Phone { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        [MaxLength(20)]
        public string Gender { get; set; } = string.Empty;
        [MaxLength(100)]
        public string Specialty { get; set; } = string.Empty;
        public bool IsEmailVerified { get; set; } = false;
        public bool MustChangePassword { get; set; } = true;
        public string? VerifyToken { get; set; }
        public DateTime? VerifyTokenExpiry { get; set; }
        // Foreign Keys
        public int RoleId { get; set; }
        // Navigation Properties
        [ForeignKey("RoleId")]
        public virtual Role Role { get; set; } = null!;

        // Navigation Properties (bổ sung theo ERD)
        public virtual ICollection<ChildProfile> ChildProfiles { get; set; } = new List<ChildProfile>();
        public virtual ICollection<Classroom> Classrooms { get; set; } = new List<Classroom>();
        public virtual ICollection<Semester> Semesters { get; set; } = new List<Semester>();
        public virtual ICollection<Exercise> Exercises { get; set; } = new List<Exercise>();
        public virtual ICollection<ExerciseQuestion> ExerciseQuestions { get; set; } = new List<ExerciseQuestion>();
        public virtual ICollection<Report> Reports { get; set; } = new List<Report>();
    }
}