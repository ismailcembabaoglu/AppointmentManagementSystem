using System.ComponentModel.DataAnnotations;

namespace AppointmentManagementSystem.Domain.Entities
{
    public class AppointmentServiceItem : BaseEntity
    {
        [Required]
        public int AppointmentId { get; set; }

        [Required]
        public int ServiceId { get; set; }

        [Required]
        [MaxLength(200)]
        public string ServiceName { get; set; } = string.Empty;

        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        [Range(1, 1440)]
        public int DurationMinutes { get; set; }

        public virtual Appointment? Appointment { get; set; }
        public virtual Service? Service { get; set; }
    }
}
