using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GodotXR.Domain.Entities
{
    public abstract class BaseEntity
    {
        // Giờ Việt Nam = UTC+7 (không phụ thuộc timezone database của OS)
        private static DateTime VietnamNow => DateTime.UtcNow.AddHours(7);

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public DateTime CreatedAt { get; set; } = VietnamNow;
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAt { get; set; }
    }
}
