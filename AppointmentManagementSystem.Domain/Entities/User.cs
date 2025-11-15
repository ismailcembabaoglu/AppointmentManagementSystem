using System.ComponentModel.DataAnnotations;

namespace AppointmentManagementSystem.Domain.Entities
{
    public class User : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Role { get; set; } = string.Empty; // "Customer", "Business", "Admin"

        // Ortak alanlar
        [MaxLength(20)]
        public string? PhoneNumber { get; set; }

        public DateTime? DateOfBirth { get; set; }

        [MaxLength(500)]
        public string? ProfilePhotoPath { get; set; }

        // Customer için alanlar
        [MaxLength(500)]
        public string? Address { get; set; }

        [MaxLength(100)]
        public string? City { get; set; }

        // Business için alanlar (şirket sahibi ise)
        public int? OwnedBusinessId { get; set; }
        public virtual Business? OwnedBusiness { get; set; }

        public bool IsActive { get; set; } = false; // Email doğrulanana kadar false

        // Email Verification
        public bool IsEmailVerified { get; set; } = false;
        
        [MaxLength(255)]
        public string? EmailVerificationToken { get; set; }
        
        public DateTime? EmailVerificationTokenExpiry { get; set; }

        // Navigation properties
        public virtual ICollection<BusinessUser>? BusinessUsers { get; set; } 
        public virtual ICollection<Appointment>? CustomerAppointments { get; set; }
    }
}
