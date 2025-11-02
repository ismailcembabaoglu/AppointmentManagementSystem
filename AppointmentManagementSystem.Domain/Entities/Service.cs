using System.ComponentModel.DataAnnotations;

namespace AppointmentManagementSystem.Domain.Entities
{
    public class Service : BaseEntity
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? Price { get; set; }

        [Required]
        [Range(1, 1440)]
        public int DurationMinutes { get; set; }

        [Required]
        public int BusinessId { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual Business? Business { get; set; }
        public virtual ICollection<ServicePhoto>? Photos { get; set; } 
        public virtual ICollection<Appointment>? Appointments { get; set; } 
    }
}
