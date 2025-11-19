using AppointmentManagementSystem.Application.DTOs;
using FluentValidation;

namespace AppointmentManagementSystem.Application.Validators
{
    public class CreateBusinessDtoValidator : AbstractValidator<CreateBusinessDto>
    {
        public CreateBusinessDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("İşletme adı gereklidir.")
                .MaximumLength(200).WithMessage("İşletme adı en fazla 200 karakter olabilir.");

            RuleFor(x => x.CategoryId)
                .GreaterThan(0).WithMessage("Geçerli bir kategori seçmelisiniz.");

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("Açıklama en fazla 1000 karakter olabilir.");

            RuleFor(x => x.Address)
                .MaximumLength(500).WithMessage("Adres en fazla 500 karakter olabilir.")
                .NotEmpty().WithMessage("Adress Alanı boş geçilemez");

            RuleFor(x => x.City)
                .MaximumLength(100).WithMessage("İl en fazla 100 karakter olabilir.")
                .NotEmpty().WithMessage("İl Alanı boş geçilemez"); ;

            RuleFor(x => x.District)
                .MaximumLength(100).WithMessage("İlçe en fazla 100 karakter olabilir.")
                .NotEmpty().WithMessage("İlçe Alanı boş geçilemez"); ;

            RuleFor(x => x.Phone)
                .MaximumLength(20).WithMessage("Telefon numarası en fazla 20 karakter olabilir.")
                .Matches(@"^[\d\s\(\)\+\-]*$").WithMessage("Geçersiz telefon numarası formatı.")
                .When(x => !string.IsNullOrEmpty(x.Phone));

            RuleFor(x => x.Email)
                .EmailAddress().WithMessage("Geçersiz e-posta adresi.")
                .MaximumLength(100).WithMessage("E-posta adresi en fazla 100 karakter olabilir.")
                .When(x => !string.IsNullOrEmpty(x.Email));

            RuleFor(x => x.Website)
                .MaximumLength(200).WithMessage("Web sitesi adresi en fazla 200 karakter olabilir.")
                .When(x => !string.IsNullOrEmpty(x.Website));
        }
    }
}
