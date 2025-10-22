using System.ComponentModel.DataAnnotations;

namespace AppointmentManagementSystem.BlazorUI.Models
{
    public class LoginDto
    {
        [Required(ErrorMessage = "E-posta alanı gereklidir.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifre alanı gereklidir.")]
        public string Password { get; set; } = string.Empty;
    }
}
