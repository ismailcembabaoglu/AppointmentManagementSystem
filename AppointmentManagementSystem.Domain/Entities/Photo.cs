using System.ComponentModel.DataAnnotations;

namespace AppointmentManagementSystem.Domain.Entities
{
    public abstract class Photo : BaseEntity
    {
        [Required]
        [MaxLength(500)]
        public string FileName { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? FilePath { get; set; }

        [MaxLength(100)]
        public string? ContentType { get; set; }

        public long FileSize { get; set; }
    }
}
