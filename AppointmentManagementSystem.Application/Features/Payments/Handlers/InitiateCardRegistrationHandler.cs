using AppointmentManagementSystem.Application.DTOs;
using AppointmentManagementSystem.Application.Features.Payments.Commands;
using AppointmentManagementSystem.Application.Shared;
using AppointmentManagementSystem.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace AppointmentManagementSystem.Application.Features.Payments.Handlers
{
    public class InitiateCardRegistrationHandler : IRequestHandler<InitiateCardRegistrationCommand, Result<CardRegistrationResponseDto>>
    {
        private readonly IPayTRService _paytrService;
        private readonly ILogger<InitiateCardRegistrationHandler> _logger;

        public InitiateCardRegistrationHandler(IPayTRService paytrService, ILogger<InitiateCardRegistrationHandler> logger)
        {
            _paytrService = paytrService;
            _logger = logger;
        }

        public async Task<Result<CardRegistrationResponseDto>> Handle(InitiateCardRegistrationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // PayTR e-posta ve açıklama için güvenli değerler üret
                var safeEmail = SanitizeEmail(request.Email, request.BusinessName, request.BusinessId);
                var safeDescription = string.IsNullOrWhiteSpace(request.Description)
                    ? "Kart Doğrulama Ücreti"
                    : request.Description.Trim();
                var safeAmount = request.Amount < 1m ? 1m : request.Amount;
                var safeIp = string.IsNullOrWhiteSpace(request.UserIp) ? "127.0.0.1" : request.UserIp;

                // Generate merchant_oid: REG{BusinessId}_{Guid}
                var guidPart = Guid.NewGuid().ToString("N").Substring(0, 8);
                var prefix = request.IsCardUpdate ? "CARD" : "REG";
                var merchantOid = $"{prefix}{request.BusinessId}_{guidPart}";

                var result = await _paytrService.InitiateCardRegistrationAsync(
                    safeEmail,
                    safeIp,
                    merchantOid,
                    safeAmount,
                    safeDescription,
                    request.IsCardUpdate
                );

                if (result.Success)
                {
                    var response = new CardRegistrationResponseDto
                    {
                        Success = true,
                        IFrameToken = result.IFrameToken,
                        IFrameUrl = result.IFrameUrl
                    };

                    return Result<CardRegistrationResponseDto>.SuccessResult(response);
                }
                else
                {
                    return Result<CardRegistrationResponseDto>.FailureResult(result.ErrorMessage ?? "Card registration failed");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in InitiateCardRegistrationHandler");
                return Result<CardRegistrationResponseDto>.FailureResult("An error occurred during card registration");
            }
        }

        private static string SanitizeEmail(string? email, string businessName, int businessId)
        {
            var trimmed = email?.Trim() ?? string.Empty;
            var looksValid = trimmed.Contains('@') && trimmed.Contains('.') && !trimmed.EndsWith('@');

            if (looksValid)
            {
                return trimmed;
            }

            // Basit bir fallback e-posta üretimi (PayTR parametre hatasını önlemek için)
            var safeName = string.IsNullOrWhiteSpace(businessName) ? "business" : businessName;
            var slug = new string(safeName.ToLowerInvariant().Where(char.IsLetterOrDigit).ToArray());
            if (string.IsNullOrWhiteSpace(slug))
            {
                slug = "business";
            }

            return $"{slug}{businessId}@aptivaplan.local";
        }
    }
}