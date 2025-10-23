using System.ComponentModel.DataAnnotations;

namespace AppointmentManagementSystem.Domain.Entities
{
    public class BusinessUser : BaseEntity
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

        // Navigation properties
        public virtual Business? Business { get; set; } = new();
        public virtual User? User { get; set; } = new();
        public virtual ICollection<Employee>? ManagedEmployees { get; set; } = new List<Employee>();
    }
}
