using System;
using AppointmentManagementSystem.Application.Features.Payments.Commands;
using AppointmentManagementSystem.Application.Interfaces;
using AppointmentManagementSystem.Application.Shared;
using AppointmentManagementSystem.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AppointmentManagementSystem.Application.Features.Payments.Handlers
{
    public class ChargeManualBillingHandler : IRequestHandler<ChargeManualBillingCommand, Result<ChargeManualBillingResponse>>
    {
        private readonly IPayTRDirectAPIService _paytrDirectService;
        private readonly IBusinessSubscriptionRepository _subscriptionRepository;
        private readonly IBusinessRepository _businessRepository;
        private readonly ILogger<ChargeManualBillingHandler> _logger;

        public ChargeManualBillingHandler(
            IPayTRDirectAPIService paytrDirectService,
            IBusinessSubscriptionRepository subscriptionRepository,
            IBusinessRepository businessRepository,
            ILogger<ChargeManualBillingHandler> logger)
        {
            _paytrDirectService = paytrDirectService;
            _subscriptionRepository = subscriptionRepository;
            _businessRepository = businessRepository;
            _logger = logger;
        }

        public async Task<Result<ChargeManualBillingResponse>> Handle(ChargeManualBillingCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation($"=== Manual Billing (Direct API) Started for Business {request.BusinessId} ===");

                var subscription = await _subscriptionRepository.GetByBusinessIdAsync(request.BusinessId);
                if (subscription == null || string.IsNullOrWhiteSpace(subscription.PayTRUserToken) || string.IsNullOrWhiteSpace(subscription.PayTRCardToken))
                {
                    return Result<ChargeManualBillingResponse>.FailureResult("Abonelik veya kayıtlı kart bulunamadı.");
                }

                var business = await _businessRepository.GetByIdAsync(request.BusinessId);
                if (business == null)
                {
                    return Result<ChargeManualBillingResponse>.FailureResult("İşletme bulunamadı.");
                }

                var amount = subscription.MonthlyAmount > 0 ? subscription.MonthlyAmount : 700m;
                var merchantOid = $"BILL{request.BillingYear}{request.BillingMonth:D2}{request.BusinessId}{Guid.NewGuid().ToString("N")[..6].ToUpper()}";

                var response = await _paytrDirectService.ChargeStoredCard(
                    subscription.PayTRUserToken,
                    subscription.PayTRCardToken,
                    business.Email ?? string.Empty,
                    business.Name ?? string.Empty,
                    amount,
                    merchantOid,
                    request.UserIp);

                if (!response.Success)
                {
                    _logger.LogError($"❌ Manual billing failed: {response.ErrorMessage}");
                    return Result<ChargeManualBillingResponse>.FailureResult(response.ErrorMessage ?? "Ödeme başlatılamadı.");
                }

                _logger.LogInformation($"✅ Manual billing initiated. MerchantOid: {merchantOid}");

                return Result<ChargeManualBillingResponse>.SuccessResult(new ChargeManualBillingResponse
                {
                    Success = true,
                    MerchantOid = merchantOid,
                    Message = "Ödeme başlatıldı, webhook bekleniyor."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error charging manual billing via Direct API");
                return Result<ChargeManualBillingResponse>.FailureResult(ex.Message);
            }
        }
    }
}
