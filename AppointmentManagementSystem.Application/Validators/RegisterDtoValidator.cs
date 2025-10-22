using AppointmentManagementSystem.Application.DTOs;
using FluentValidation;

namespace AppointmentManagementSystem.Application.Validators
{
    public class RegisterDtoValidator : AbstractValidator<RegisterDto>
    {
        public RegisterDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("İsim alanı boş olamaz.")
                .MaximumLength(100).WithMessage("İsim en fazla 100 karakter olabilir.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("E-posta alanı boş olamaz.")
                .EmailAddress().WithMessage("Geçerli bir e-posta adresi giriniz.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Şifre alanı boş olamaz.")
                .MinimumLength(6).WithMessage("Şifre en az 6 karakter olmalıdır.");

            RuleFor(x => x.Role)
                .NotEmpty().WithMessage("Rol alanı boş olamaz.")
                .Must(role => role == "Customer" || role == "Business")
                .WithMessage("Rol 'Customer' veya 'Business' olmalıdır.");
        }
    }
}
