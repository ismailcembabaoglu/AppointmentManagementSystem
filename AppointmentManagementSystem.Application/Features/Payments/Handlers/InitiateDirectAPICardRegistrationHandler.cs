using AppointmentManagementSystem.Application.Features.Payments.Commands;
using AppointmentManagementSystem.Application.Interfaces;
using AppointmentManagementSystem.Application.Shared;
using AppointmentManagementSystem.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AppointmentManagementSystem.Application.Features.Payments.Handlers
{
    public class InitiateDirectAPICardRegistrationHandler
        : IRequestHandler<InitiateDirectAPICardRegistrationCommand, Result<InitiateDirectAPICardRegistrationResponse>>
    {
        private readonly IPayTRDirectAPIService _paytrDirectService;
        private readonly IBusinessRepository _businessRepository;
        private readonly IBusinessSubscriptionRepository _subscriptionRepository;
        private readonly ILogger<InitiateDirectAPICardRegistrationHandler> _logger;

        public InitiateDirectAPICardRegistrationHandler(
            IPayTRDirectAPIService paytrDirectService,
            IBusinessRepository businessRepository,
            IBusinessSubscriptionRepository subscriptionRepository,
            ILogger<InitiateDirectAPICardRegistrationHandler> logger)
        {
            _paytrDirectService = paytrDirectService;
            _businessRepository = businessRepository;
            _subscriptionRepository = subscriptionRepository;
            _logger = logger;
        }

        public async Task<Result<InitiateDirectAPICardRegistrationResponse>> Handle(
            InitiateDirectAPICardRegistrationCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation($"=== Direct API Card Registration Started ===");
                _logger.LogInformation($"BusinessId: {request.BusinessId}, Email: {request.Email}");

                // İşletme kontrolü
                var business = await _businessRepository.GetByIdAsync(request.BusinessId);
                if (business == null)
                {
                    return Result<InitiateDirectAPICardRegistrationResponse>.FailureResult("Business not found");
                }

                // Mevcut subscription kontrolü (varsa utoken'ı alacağız)
                var existingSubscription = await _subscriptionRepository.GetByBusinessIdAsync(request.BusinessId);
                string? existingUtoken = existingSubscription?.PayTRUserToken;

                if (!string.IsNullOrEmpty(existingUtoken))
                {
                    _logger.LogInformation($"Existing UToken found: {existingUtoken.Substring(0, Math.Min(10, existingUtoken.Length))}... (Adding another card to same user)");
                }

                // Merchant OID oluştur (REG prefix ile webhook'ta tanıyabiliriz)
                var merchantOid = $"REG{request.BusinessId}_{Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper()}";

                // Direct API ile ödeme başlat
                var paymentResponse = await _paytrDirectService.InitiateCardRegistrationPayment(
                    businessId: request.BusinessId,
                    email: request.Email,
                    userName: request.OwnerName,
                    userAddress: request.Address,
                    userPhone: request.PhoneNumber,
                    ccOwner: request.CardOwner,
                    cardNumber: request.CardNumber,
                    expiryMonth: request.ExpiryMonth,
                    expiryYear: request.ExpiryYear,
                    cvv: request.CVV,
                    amount: 700.00m, // İlk ödeme 700 TL
                    merchantOid: merchantOid,
                    userIp: request.UserIp,
                    existingUtoken: existingUtoken
                );

                if (!paymentResponse.Success)
                {
                    _logger.LogError($"❌ Payment failed: {paymentResponse.ErrorMessage}");
                    return Result<InitiateDirectAPICardRegistrationResponse>.FailureResult(
                        paymentResponse.ErrorMessage ?? "Payment initiation failed");
                }

                _logger.LogInformation($"✅ Direct API payment initiated successfully");
                _logger.LogInformation($"MerchantOid: {merchantOid}");
                _logger.LogInformation($"⏳ Waiting for webhook callback with card tokens (utoken/ctoken)...");

                return Result<InitiateDirectAPICardRegistrationResponse>.SuccessResult(
                    new InitiateDirectAPICardRegistrationResponse
                    {
                        Success = true,
                        MerchantOid = merchantOid,
                        Message = "Payment processing. Webhook will confirm card storage.",
                        RedirectUrl = paymentResponse.PaymentUrl // Null olacak (non-3D)
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error in InitiateDirectAPICardRegistration");
                return Result<InitiateDirectAPICardRegistrationResponse>.FailureResult(ex.Message);
            }
        }
    }
}
