using AppointmentManagementSystem.Application.DTOs;
using FluentValidation;

namespace AppointmentManagementSystem.Application.Validators
{
    public class CreateEmployeeDtoValidator : AbstractValidator<CreateEmployeeDto>
    {
        public CreateEmployeeDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Çalışan adı gereklidir.")
                .MaximumLength(200).WithMessage("Çalışan adı en fazla 200 karakter olabilir.");

            RuleFor(x => x.BusinessId)
                .GreaterThan(0).WithMessage("Geçerli bir işletme seçmelisiniz.");

            RuleFor(x => x.Phone)
                .MaximumLength(20).WithMessage("Telefon numarası en fazla 20 karakter olabilir.")
                .Matches(@"^[\d\s\(\)\+\-]*$").WithMessage("Geçersiz telefon numarası formatı.")
                .When(x => !string.IsNullOrEmpty(x.Phone));

            RuleFor(x => x.Email)
                .EmailAddress().WithMessage("Geçersiz e-posta adresi.")
                .MaximumLength(100).WithMessage("E-posta adresi en fazla 100 karakter olabilir.")
                .When(x => !string.IsNullOrEmpty(x.Email));

            RuleFor(x => x.Title)
                .MaximumLength(100).WithMessage("Unvan en fazla 100 karakter olabilir.")
                .When(x => !string.IsNullOrEmpty(x.Title));

            RuleFor(x => x.Specialization)
                .MaximumLength(200).WithMessage("Uzmanlık alanı en fazla 200 karakter olabilir.")
                .When(x => !string.IsNullOrEmpty(x.Specialization));
        }
    }
}
