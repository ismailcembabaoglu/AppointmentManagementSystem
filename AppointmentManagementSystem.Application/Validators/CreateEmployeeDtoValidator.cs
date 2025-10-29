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
                .MaximumLength(100).WithMessage("Çalışan adı en fazla 100 karakter olabilir.");

            RuleFor(x => x.BusinessId)
                .GreaterThan(0).WithMessage("Geçerli bir işletme seçmelisiniz.");

            RuleFor(x => x.Specialization)
                .MaximumLength(200).WithMessage("Uzmanlık alanı en fazla 200 karakter olabilir.")
                .When(x => !string.IsNullOrEmpty(x.Specialization));

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Açıklama en fazla 500 karakter olabilir.")
                .When(x => !string.IsNullOrEmpty(x.Description));
        }
    }
}
