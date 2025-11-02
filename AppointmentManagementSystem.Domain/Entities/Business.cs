using System.ComponentModel.DataAnnotations;

namespace AppointmentManagementSystem.Domain.Entities
{
    public class Business : BaseEntity
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public int CategoryId { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

        [MaxLength(500)]
        public string? Address { get; set; }

        [MaxLength(20)]
        public string? Phone { get; set; }

        [MaxLength(200)]
        public string? Email { get; set; }

        [MaxLength(200)]
        public string? Website { get; set; }

        [MaxLength(100)]
        public string? City { get; set; }

        [MaxLength(100)]
        public string? District { get; set; }

        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual Category? Category { get; set; } 
        public virtual ICollection<BusinessUser>? BusinessUsers { get; set; } 
        public virtual ICollection<Service>? Services { get; set; }
        public virtual ICollection<Employee>? Employees { get; set; } 
        public virtual ICollection<Appointment>? Appointments { get; set; } 
        public virtual ICollection<BusinessPhoto>? Photos { get; set; } 
    }
}
