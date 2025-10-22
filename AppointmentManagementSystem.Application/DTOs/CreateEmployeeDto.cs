using System.ComponentModel.DataAnnotations;

namespace AppointmentManagementSystem.Application.DTOs
{
    public class CreateEmployeeDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? Specialization { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        public int BusinessId { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
