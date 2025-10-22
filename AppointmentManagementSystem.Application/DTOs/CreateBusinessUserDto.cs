using System.ComponentModel.DataAnnotations;

namespace AppointmentManagementSystem.Application.DTOs
{
    public class CreateBusinessUserDto
    {
        [Required]
        public int BusinessId { get; set; }

        [Required]
        public int UserId { get; set; }

        [MaxLength(100)]
        public string? Position { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        public bool CanCreateAppointments { get; set; } = false;
        public bool CanManageServices { get; set; } = false;
        public bool CanManageEmployees { get; set; } = false;
    }
}
