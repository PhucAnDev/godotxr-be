using GodotXR.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace GodotXR.Domain.Entities
{
    public class Role : BaseEntity
    {
        [Required]
        public UserRole RoleName { get; set; }

        [MaxLength(250)]
        public string Description { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public virtual ICollection<User> Users { get; set; } = new List<User>();
    }
}
