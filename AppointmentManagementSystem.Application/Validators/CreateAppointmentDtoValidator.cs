using AppointmentManagementSystem.Application.DTOs;
using FluentValidation;
using System.Linq;

namespace AppointmentManagementSystem.Application.Validators
{
    public class CreateAppointmentDtoValidator : AbstractValidator<CreateAppointmentDto>
    {
        public CreateAppointmentDtoValidator()
        {
            RuleFor(x => x.CustomerId)
                .GreaterThan(0).WithMessage("Geçerli bir müşteri seçmelisiniz.");

            RuleFor(x => x.BusinessId)
                .GreaterThan(0).WithMessage("Geçerli bir işletme seçmelisiniz.");

            RuleFor(x => x)
                .Must(x => (x.ServiceIds != null && x.ServiceIds.Any()) || x.ServiceId > 0)
                .WithMessage("Geçerli en az bir hizmet seçmelisiniz.");

            RuleFor(x => x.AppointmentDate)
                .NotEmpty().WithMessage("Randevu tarihi gereklidir.")
                .GreaterThanOrEqualTo(DateTime.Today).WithMessage("Geçmiş tarih seçilemez.");

            RuleFor(x => x.StartTime)
                .NotEmpty().WithMessage("Başlangıç saati gereklidir.");

            RuleFor(x => x.Notes)
                .MaximumLength(1000).WithMessage("Notlar en fazla 1000 karakter olabilir.")
                .When(x => !string.IsNullOrEmpty(x.Notes));
        }
    }
}
