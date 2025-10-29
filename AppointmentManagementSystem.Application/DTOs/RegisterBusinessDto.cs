using System.ComponentModel.DataAnnotations;

namespace AppointmentManagementSystem.Application.DTOs
{
    public class RegisterBusinessDto
    {
        // User Information
        [Required(ErrorMessage = "Ad Soyad alanı gereklidir.")]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "E-posta alanı gereklidir.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifre alanı gereklidir.")]
        [MinLength(6, ErrorMessage = "Şifre en az 6 karakter olmalıdır.")]
        public string Password { get; set; } = string.Empty;

        // Business Information
        [Required(ErrorMessage = "İşletme adı gereklidir.")]
        [MaxLength(200)]
        public string BusinessName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Kategori seçimi gereklidir.")]
        public int CategoryId { get; set; }

        [MaxLength(1000)]
        public string? BusinessDescription { get; set; }

        [MaxLength(500)]
        public string? Address { get; set; }

        [MaxLength(20)]
        public string? Phone { get; set; }

        [MaxLength(200)]
        public string? BusinessEmail { get; set; }

        [MaxLength(200)]
        public string? Website { get; set; }

        [MaxLength(100)]
        public string? City { get; set; }

        [MaxLength(100)]
        public string? District { get; set; }

        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        // Business Photo (Base64)
        public string? BusinessPhotoBase64 { get; set; }

        // Services
        public List<RegisterBusinessServiceDto> Services { get; set; } = new();

        // Employees
        public List<RegisterBusinessEmployeeDto> Employees { get; set; } = new();
    }

    public class RegisterBusinessServiceDto
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        public decimal? Price { get; set; }

        [Required]
        public int DurationMinutes { get; set; }
    }

    public class RegisterBusinessEmployeeDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? Specialization { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        // Employee Photos (Base64)
        public List<string> PhotosBase64 { get; set; } = new();
    }
}
