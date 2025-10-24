using System.ComponentModel.DataAnnotations;

namespace AppointmentManagementSystem.Domain.Entities
{
    public class EmployeeDocument : BaseEntity
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        [MaxLength(500)]
        public string FileName { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? FilePath { get; set; }

        [MaxLength(100)]
        public string? ContentType { get; set; }

        public long FileSize { get; set; }

        /// <summary>
        /// Base64 encoded document data for storing documents directly in database
        /// </summary>
        public string? Base64Data { get; set; }

        public int EmployeeId { get; set; }

        // Navigation property
        public virtual Employee? Employee { get; set; } = new();
    }
}
