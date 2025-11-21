using AppointmentManagementSystem.Application.DTOs;
using FluentValidation;

namespace AppointmentManagementSystem.Application.Validators
{
    public class UploadPhotoDtoValidator : AbstractValidator<UploadPhotoDto>
    {
        public UploadPhotoDtoValidator()
        {
            RuleFor(x => x.FileName)
                .NotEmpty().WithMessage("Dosya adı gereklidir.")
                .MaximumLength(500).WithMessage("Dosya adı en fazla 500 karakter olabilir.");

            RuleFor(x => x.Base64Data)
                .NotEmpty().WithMessage("Dosya verisi gereklidir.")
                .Must(BeValidBase64).WithMessage("Geçersiz dosya verisi.");

            RuleFor(x => x.ContentType)
                .Must(BeValidImageType).WithMessage("Sadece JPEG, PNG ve WebP formatları desteklenir.")
                .When(x => !string.IsNullOrEmpty(x.ContentType));
        }

        private bool BeValidBase64(string base64)
        {
            if (string.IsNullOrEmpty(base64))
                return false;

            try
            {
                var normalized = NormalizeBase64(base64);
                Convert.FromBase64String(normalized);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static string NormalizeBase64(string base64)
        {
            var markerIndex = base64.IndexOf("base64,", StringComparison.OrdinalIgnoreCase);
            return markerIndex >= 0
                ? base64[(markerIndex + "base64,".Length)..]
                : base64;
        }

        private bool BeValidImageType(string contentType)
        {
            var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/webp" };
            return allowedTypes.Contains(contentType.ToLower());
        }
    }
}
