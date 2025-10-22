using System.ComponentModel.DataAnnotations;

namespace AppointmentManagementSystem.Domain.Entities
{
    public class Employee : BaseEntity
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

        public int? BusinessUserId { get; set; } // Çalışan aynı zamanda BusinessUser olabilir

        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual Business Business { get; set; } = new();
        public virtual BusinessUser? BusinessUser { get; set; }
        public virtual ICollection<EmployeePhoto> Photos { get; set; } = new List<EmployeePhoto>();
        public virtual ICollection<EmployeeDocument> Documents { get; set; } = new List<EmployeeDocument>();
        public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}
