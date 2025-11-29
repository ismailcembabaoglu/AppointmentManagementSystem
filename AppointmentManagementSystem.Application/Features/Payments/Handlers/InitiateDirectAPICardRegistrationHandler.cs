using AppointmentManagementSystem.Application.Features.Payments.Commands;
using AppointmentManagementSystem.Application.Interfaces;
using AppointmentManagementSystem.Application.Shared;
using AppointmentManagementSystem.Domain.Entities;
using AppointmentManagementSystem.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace AppointmentManagementSystem.Application.Features.Payments.Handlers
{
    public class InitiateDirectAPICardRegistrationHandler
        : IRequestHandler<InitiateDirectAPICardRegistrationCommand, Result<InitiateDirectAPICardRegistrationResponse>>
    {
        private readonly IPayTRDirectAPIService _paytrDirectService;
        private readonly IBusinessRepository _businessRepository;
        private readonly IBusinessSubscriptionRepository _subscriptionRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<InitiateDirectAPICardRegistrationHandler> _logger;

        public InitiateDirectAPICardRegistrationHandler(
            IPayTRDirectAPIService paytrDirectService,
            IBusinessRepository businessRepository,
            IBusinessSubscriptionRepository subscriptionRepository,
            IUnitOfWork unitOfWork,
            ILogger<InitiateDirectAPICardRegistrationHandler> logger)
        {
            _paytrDirectService = paytrDirectService;
            _businessRepository = businessRepository;
            _subscriptionRepository = subscriptionRepository;
            _unitOfWork = unitOfWork;
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
                // PayTR tire (-) kabul etmiyor, sadece harf ve rakam
                var merchantOid = $"REG{request.BusinessId}{Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper()}";

                // Direct API ile ödeme başlat
                // İlk kayıtta 1 TL kart doğrulama ücreti alınır
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
                    amount: 1.00m, // Kart doğrulama ücreti: 1 TL
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

                var maskedPan = paymentResponse.MaskedPan ?? MaskCardNumber(request.CardNumber);
                var cardBrand = paymentResponse.CardBrand ?? DetectCardBrand(request.CardNumber);

                var subscription = await SaveOrUpdateSubscriptionAsync(
                    request.BusinessId,
                    paymentResponse.UserToken,
                    paymentResponse.CardToken,
                    maskedPan,
                    cardBrand);

                // PayTR "Kayıtlı Karttan Ödeme" dokümantasyonu gereği, kart kaydedildikten sonra
                // utoken/ctoken ile 1 TL doğrulama çekimi yapıyoruz.
                var userToken = subscription.PayTRUserToken ?? paymentResponse.UserToken;
                var cardToken = subscription.PayTRCardToken ?? paymentResponse.CardToken;

                if (string.IsNullOrWhiteSpace(userToken))
                {
                    _logger.LogError("❌ UToken not found after card registration; cannot perform verification charge");
                    return Result<InitiateDirectAPICardRegistrationResponse>.FailureResult(
                        "Kart kaydedildi ancak kullanıcı token'ı alınamadı. Lütfen tekrar deneyin.");
                }

                // Eğer kart token yoksa kart listesinden çekmeyi dene
                if (string.IsNullOrWhiteSpace(cardToken))
                {
                    var cardList = await _paytrDirectService.GetStoredCards(userToken);
                    if (cardList.Success && cardList.Cards?.Any() == true)
                    {
                        var matchedCard = cardList.Cards
                            .FirstOrDefault(c => !string.IsNullOrWhiteSpace(c.MaskedPan) && c.MaskedPan == maskedPan)
                            ?? cardList.Cards.First();

                        cardToken = matchedCard.Ctoken;

                        if (!string.IsNullOrWhiteSpace(cardToken) && subscription.PayTRCardToken != cardToken)
                        {
                            subscription.PayTRCardToken = cardToken;
                            await _subscriptionRepository.UpdateAsync(subscription);
                            await _unitOfWork.SaveChangesAsync();
                        }
                    }
                }

                if (string.IsNullOrWhiteSpace(cardToken))
                {
                    _logger.LogError("❌ Card token not found; skipping verification charge");
                    return Result<InitiateDirectAPICardRegistrationResponse>.FailureResult(
                        "Kart kaydedildi ancak doğrulama çekimi için kart token'ı alınamadı.");
                }

                var verifyOid = $"VRF{request.BusinessId}{Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper()}";
                var chargeResponse = await _paytrDirectService.ChargeStoredCard(
                    utoken: userToken,
                    ctoken: cardToken,
                    email: request.Email,
                    userName: request.OwnerName,
                    amount: 1.00m, // 1 TL doğrulama çekimi
                    merchantOid: verifyOid,
                    userIp: request.UserIp,
                    cvv: request.CVV
                );

                if (!chargeResponse.Success)
                {
                    _logger.LogError($"❌ Verification charge failed: {chargeResponse.ErrorMessage}");
                    return Result<InitiateDirectAPICardRegistrationResponse>.FailureResult(
                        chargeResponse.ErrorMessage ?? "Kart doğrulama çekimi başarısız oldu.");
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
                        RedirectUrl = paymentResponse.PaymentUrl, // Null olacak (non-3D)
                        PayTRUserToken = paymentResponse.UserToken,
                        PayTRCardToken = paymentResponse.CardToken,
                        MaskedCardNumber = maskedPan,
                        CardBrand = cardBrand
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error in InitiateDirectAPICardRegistration");
                return Result<InitiateDirectAPICardRegistrationResponse>.FailureResult(ex.Message);
            }
        }

        private async Task<BusinessSubscription> SaveOrUpdateSubscriptionAsync(
            int businessId,
            string? userToken,
            string? cardToken,
            string? maskedPan,
            string? cardBrand)
        {
            var subscription = await _subscriptionRepository.GetByBusinessIdAsync(businessId);
            var lastFour = maskedPan != null
                ? new string(maskedPan.Where(char.IsDigit).TakeLast(4).ToArray())
                : null;

            if (subscription == null)
            {
                subscription = new BusinessSubscription
                {
                    BusinessId = businessId,
                    PayTRUserToken = userToken,
                    PayTRCardToken = cardToken,
                    CardBrand = cardBrand,
                    CardType = cardBrand,
                    MaskedCardNumber = maskedPan,
                    CardLastFourDigits = lastFour,
                    MonthlyAmount = 700.00m,
                    Status = SubscriptionStatus.PendingPayment,
                    SubscriptionStatus = SubscriptionStatus.PendingPayment,
                    StartDate = DateTime.UtcNow,
                    SubscriptionStartDate = DateTime.UtcNow,
                    AutoRenewal = true,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    NextBillingDate = DateTime.UtcNow.AddDays(30)
                };

                await _subscriptionRepository.AddAsync(subscription);
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(userToken))
                {
                    subscription.PayTRUserToken = userToken;
                }

                if (!string.IsNullOrWhiteSpace(cardToken))
                {
                    subscription.PayTRCardToken = cardToken;
                }

                subscription.CardBrand = cardBrand ?? subscription.CardBrand;
                subscription.CardType = cardBrand ?? subscription.CardType;
                subscription.MaskedCardNumber = maskedPan ?? subscription.MaskedCardNumber;
                subscription.CardLastFourDigits = lastFour ?? subscription.CardLastFourDigits;
                subscription.SubscriptionStatus = subscription.SubscriptionStatus ?? SubscriptionStatus.PendingPayment;
                subscription.Status = subscription.Status ?? SubscriptionStatus.PendingPayment;
                subscription.UpdatedAt = DateTime.UtcNow;

                await _subscriptionRepository.UpdateAsync(subscription);
            }

            await _unitOfWork.SaveChangesAsync();

            return subscription;
        }

        private static string? MaskCardNumber(string cardNumber)
        {
            var digits = new string((cardNumber ?? string.Empty).Where(char.IsDigit).ToArray());

            if (digits.Length < 4)
            {
                return null;
            }

            var lastFour = digits[^4..];
            return $"**** **** **** {lastFour}";
        }

        private static string? DetectCardBrand(string cardNumber)
        {
            var digits = new string((cardNumber ?? string.Empty).Where(char.IsDigit).ToArray());

            if (string.IsNullOrWhiteSpace(digits))
            {
                return null;
            }

            return digits[0] switch
            {
                '4' => "Visa",
                '5' => "Mastercard",
                '3' => "American Express",
                '6' => "Discover",
                _ => "Bilinmiyor"
            };
        }
    }
}
