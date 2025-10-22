﻿using System.ComponentModel.DataAnnotations;

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

        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual Business? OwnedBusiness { get; set; }
        public virtual ICollection<BusinessUser> BusinessUsers { get; set; } = new List<BusinessUser>();
        public virtual ICollection<Appointment> CustomerAppointments { get; set; } = new List<Appointment>();
    }
}
