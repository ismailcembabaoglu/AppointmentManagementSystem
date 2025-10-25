using AppointmentManagementSystem.Application.DTOs;
using FluentValidation;

namespace AppointmentManagementSystem.Application.Validators
{
    public class RegisterDtoValidator : AbstractValidator<RegisterDto>
    {
        public RegisterDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Ad gereklidir.")
                .MaximumLength(200).WithMessage("Ad en fazla 200 karakter olabilir.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("E-posta adresi gereklidir.")
                .EmailAddress().WithMessage("Geçersiz e-posta adresi.")
                .MaximumLength(100).WithMessage("E-posta adresi en fazla 100 karakter olabilir.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Şifre gereklidir.")
                .MinimumLength(6).WithMessage("Şifre en az 6 karakter olmalıdır.")
                .MaximumLength(100).WithMessage("Şifre en fazla 100 karakter olabilir.");

            RuleFor(x => x.Phone)
                .MaximumLength(20).WithMessage("Telefon numarası en fazla 20 karakter olabilir.")
                .Matches(@"^[\d\s\(\)\+\-]*$").WithMessage("Geçersiz telefon numarası formatı.")
                .When(x => !string.IsNullOrEmpty(x.Phone));

            RuleFor(x => x.Role)
                .NotEmpty().WithMessage("Rol seçimi gereklidir.")
                .Must(x => x == "Customer" || x == "Business").WithMessage("Geçersiz rol. Sadece 'Customer' veya 'Business' seçilebilir.");
        }
    }
}
