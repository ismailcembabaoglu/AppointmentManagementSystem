using AppointmentManagementSystem.Application.DTOs;
using FluentValidation;

namespace AppointmentManagementSystem.Application.Validators
{
    public class CreateServiceDtoValidator : AbstractValidator<CreateServiceDto>
    {
        public CreateServiceDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Hizmet adı gereklidir.")
                .MaximumLength(200).WithMessage("Hizmet adı en fazla 200 karakter olabilir.");

            RuleFor(x => x.BusinessId)
                .GreaterThan(0).WithMessage("Geçerli bir işletme seçmelisiniz.");

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("Açıklama en fazla 1000 karakter olabilir.")
                .When(x => !string.IsNullOrEmpty(x.Description));

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Fiyat 0'dan büyük olmalıdır.")
                .LessThanOrEqualTo(1000000).WithMessage("Fiyat çok yüksek.")
                .When(x => x.Price.HasValue);

            RuleFor(x => x.DurationMinutes)
                .GreaterThan(0).WithMessage("Süre 0'dan büyük olmalıdır.")
                .LessThanOrEqualTo(1440).WithMessage("Süre maksimum 24 saat (1440 dakika) olabilir.");
        }
    }
}
