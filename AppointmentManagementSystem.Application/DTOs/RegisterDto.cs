using System.ComponentModel.DataAnnotations;

namespace AppointmentManagementSystem.Application.DTOs
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "Ad Soyad alanı gereklidir.")]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "E-posta alanı gereklidir.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifre alanı gereklidir.")]
        [MinLength(6, ErrorMessage = "Şifre en az 6 karakter olmalıdır.")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Rol seçimi gereklidir.")]
        public string Role { get; set; } = string.Empty; // "Customer" veya "Business"

        // Business rolü için ek alanlar
        public CreateBusinessDto? BusinessInfo { get; set; }
    }
}
