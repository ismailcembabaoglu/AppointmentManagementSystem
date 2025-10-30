using System.ComponentModel.DataAnnotations;

namespace AppointmentManagementSystem.Application.DTOs
{
    public class CreateAppointmentDto
    {
        [Required]
        public int CustomerId { get; set; }

        [Required]
        public int BusinessId { get; set; }

        public int? EmployeeId { get; set; }

        [Required]
        public int ServiceId { get; set; }

        [Required]
        public DateTime AppointmentDate { get; set; }

        [Required]
        public TimeSpan StartTime { get; set; }

        [MaxLength(1000)]
        public string? Notes { get; set; }
        public List<string> PhotosBase64 { get; set; } = new();
    }
}
