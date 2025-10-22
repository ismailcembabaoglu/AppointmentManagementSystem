using System.ComponentModel.DataAnnotations;

namespace AppointmentManagementSystem.BlazorUI.Models
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "İsim alanı gereklidir.")]
        [MaxLength(100, ErrorMessage = "İsim en fazla 100 karakter olabilir.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "E-posta alanı gereklidir.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifre alanı gereklidir.")]
        [MinLength(6, ErrorMessage = "Şifre en az 6 karakter olmalıdır.")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Rol alanı gereklidir.")]
        public string Role { get; set; } = string.Empty;
    }
}
