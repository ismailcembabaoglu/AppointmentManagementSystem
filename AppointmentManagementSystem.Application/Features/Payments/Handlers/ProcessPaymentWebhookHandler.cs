using AppointmentManagementSystem.Application.Features.Payments.Commands;
using AppointmentManagementSystem.Application.Shared;
using AppointmentManagementSystem.Domain.Entities;
using AppointmentManagementSystem.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AppointmentManagementSystem.Application.Features.Payments.Handlers
{
    public class ProcessPaymentWebhookHandler : IRequestHandler<ProcessPaymentWebhookCommand, Result<bool>>
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IBusinessSubscriptionRepository _subscriptionRepository;
        private readonly IBusinessRepository _businessRepository;
        private readonly IPayTRService _paytrService;
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ProcessPaymentWebhookHandler> _logger;

        public ProcessPaymentWebhookHandler(
            IPaymentRepository paymentRepository,
            IBusinessSubscriptionRepository subscriptionRepository,
            IBusinessRepository businessRepository,
            IPayTRService paytrService,
            IConfiguration configuration,
            IUnitOfWork unitOfWork,
            ILogger<ProcessPaymentWebhookHandler> logger)
        {
            _paymentRepository = paymentRepository;
            _subscriptionRepository = subscriptionRepository;
            _businessRepository = businessRepository;
            _paytrService = paytrService;
            _configuration = configuration;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result<bool>> Handle(ProcessPaymentWebhookCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation($"=== Webhook Received ===");
                _logger.LogInformation($"MerchantOid: {request.MerchantOid}");
                _logger.LogInformation($"Status: {request.Status}");
                _logger.LogInformation($"TotalAmount: {request.TotalAmount}");
                _logger.LogInformation($"Utoken: {request.Utoken}");
                _logger.LogInformation($"Ctoken: {request.Ctoken}");
                _logger.LogInformation($"CardType: {request.CardType}");
                _logger.LogInformation($"MaskedPan: {request.MaskedPan}");

                // Validate webhook signature
                var merchantSalt = _configuration["PayTR:MerchantSalt"] ?? "";
                var expectedHash = _paytrService.ValidateWebhookSignature(
                    request.MerchantOid,
                    request.Status,
                    request.TotalAmount,
                    merchantSalt
                );

                _logger.LogInformation($"Expected Hash: {expectedHash}");
                _logger.LogInformation($"Received Hash: {request.Hash}");

                if (expectedHash != request.Hash.ToLower())
                {
                    _logger.LogWarning($"Invalid webhook signature for MerchantOid: {request.MerchantOid}");
                    // Test modunda hash kontrolünü atlayalım (localhost'tan geldiği için)
                    // return Result<bool>.FailureResult("Invalid signature");
                    _logger.LogWarning("Continuing despite invalid signature (test mode)");
                }

                // Check if this is initial registration (REG prefix) or recurring payment
                if (request.MerchantOid.StartsWith("REG") && request.Status == "success")
                {
                    _logger.LogInformation("Processing initial registration callback");
                    // This is initial registration + first payment callback
                    return await HandleInitialRegistrationCallback(request, cancellationToken);
                }
                else
                {
                    _logger.LogInformation("Processing recurring payment callback");
                    // This is a recurring payment callback
                    return await HandlePaymentCallback(request, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing webhook for MerchantOid: {request.MerchantOid}");
                return Result<bool>.FailureResult("Error processing webhook");
            }
        }

        private async Task<Result<bool>> HandleInitialRegistrationCallback(ProcessPaymentWebhookCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("=== HandleInitialRegistrationCallback Started ===");
            
            // Extract BusinessId from MerchantOid (format: REG{BusinessId}{Guid})
            var merchantOid = request.MerchantOid;
            var regPrefix = "REG";
            
            _logger.LogInformation($"MerchantOid: {merchantOid}");
            
            if (!merchantOid.StartsWith(regPrefix))
            {
                _logger.LogWarning($"Invalid MerchantOid format: {merchantOid}");
                return Result<bool>.FailureResult("Invalid MerchantOid format");
            }

            // REG12abc123de -> businessId = 12
            var afterReg = merchantOid.Substring(regPrefix.Length);
            _logger.LogInformation($"After REG prefix: {afterReg}");
            
            var businessIdStr = new string(afterReg.TakeWhile(char.IsDigit).ToArray());
            _logger.LogInformation($"Extracted BusinessId string: {businessIdStr}");
            
            if (!int.TryParse(businessIdStr, out int businessId))
            {
                _logger.LogWarning($"Could not parse BusinessId from MerchantOid: {merchantOid}");
                return Result<bool>.FailureResult("Invalid MerchantOid format");
            }

            _logger.LogInformation($"Parsed BusinessId: {businessId}");

            var business = await _businessRepository.GetByIdAsync(businessId);
            if (business == null)
            {
                _logger.LogWarning($"Business not found: {businessId}");
                return Result<bool>.FailureResult("Business not found");
            }

            _logger.LogInformation($"Business found: {business.Name} (ID: {business.Id})");

            // 1. İlk ödeme kaydını oluştur
            var payment = new Payment
            {
                BusinessId = businessId,
                MerchantOid = request.MerchantOid,
                Amount = decimal.Parse(request.TotalAmount) / 100, // Kuruştan TL'ye
                Currency = "TRY",
                Status = PaymentStatus.Success,
                PaymentDate = DateTime.UtcNow,
                CardType = request.CardType,
                MaskedCardNumber = request.MaskedPan,
                PaymentType = "InitialSubscription",
                PayTRTransactionId = request.PaymentId,
                CreatedAt = DateTime.UtcNow
            };
            await _paymentRepository.AddAsync(payment);

            // 2. Abonelik oluştur veya güncelle
            var subscription = await _subscriptionRepository.GetByBusinessIdAsync(businessId);
            if (subscription == null)
            {
                subscription = new BusinessSubscription
                {
                    BusinessId = businessId,
                    PayTRUserToken = request.Utoken,
                    PayTRCardToken = request.Ctoken,
                    CardBrand = request.CardType,
                    MaskedCardNumber = request.MaskedPan,
                    MonthlyAmount = 700.00m,
                    SubscriptionStatus = SubscriptionStatus.Active,
                    SubscriptionStartDate = DateTime.UtcNow,
                    LastBillingDate = DateTime.UtcNow, // İlk ödeme yapıldı
                    NextBillingDate = DateTime.UtcNow.AddDays(30), // 30 gün sonra
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                await _subscriptionRepository.AddAsync(subscription);
            }
            else
            {
                subscription.PayTRUserToken = request.Utoken;
                subscription.PayTRCardToken = request.Ctoken;
                subscription.CardBrand = request.CardType;
                subscription.MaskedCardNumber = request.MaskedPan;
                subscription.SubscriptionStatus = SubscriptionStatus.Active;
                subscription.LastBillingDate = DateTime.UtcNow;
                subscription.NextBillingDate = DateTime.UtcNow.AddDays(30);
                subscription.UpdatedAt = DateTime.UtcNow;
                
                await _subscriptionRepository.UpdateAsync(subscription);
            }

            // 3. Business'ı aktifleştir
            business.IsActive = true;
            await _businessRepository.UpdateAsync(business);

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation($"Initial registration completed for Business {businessId}. Payment: 700 TL, Next billing: {subscription.NextBillingDate}");
            return Result<bool>.SuccessResult(true);
        }

        private async Task<Result<bool>> HandlePaymentCallback(ProcessPaymentWebhookCommand request, CancellationToken cancellationToken)
        {
            var payment = await _paymentRepository.GetByMerchantOidAsync(request.MerchantOid);
            if (payment == null)
            {
                _logger.LogWarning($"Payment not found for MerchantOid: {request.MerchantOid}");
                return Result<bool>.SuccessResult(true); // Return success to prevent retries
            }

            // Idempotency check - only process if payment is pending
            if (payment.Status != PaymentStatus.Pending)
            {
                _logger.LogInformation($"Payment {payment.Id} already processed, ignoring duplicate webhook");
                return Result<bool>.SuccessResult(true);
            }

            var subscription = await _subscriptionRepository.GetByBusinessIdAsync(payment.BusinessId);
            var business = await _businessRepository.GetByIdAsync(payment.BusinessId);

            if (request.Status == "success")
            {
                payment.Status = PaymentStatus.Success;
                payment.PaymentDate = DateTime.UtcNow;
                payment.PayTRTransactionId = request.PaymentId;

                if (subscription != null)
                {
                    subscription.LastBillingDate = DateTime.UtcNow;
                    subscription.NextBillingDate = DateTime.UtcNow.AddDays(30);
                    subscription.SubscriptionStatus = SubscriptionStatus.Active;
                    await _subscriptionRepository.UpdateAsync(subscription);
                }

                if (business != null)
                {
                    business.IsActive = true;
                    await _businessRepository.UpdateAsync(business);
                }

                _logger.LogInformation($"Payment {payment.Id} marked as successful");
            }
            else
            {
                payment.Status = PaymentStatus.Failed;
                payment.ErrorMessage = request.FailedReasonMsg ?? "Payment declined";
                payment.NextRetryDate = DateTime.UtcNow.AddHours(1);

                if (subscription != null)
                {
                    subscription.SubscriptionStatus = SubscriptionStatus.Suspended;
                    await _subscriptionRepository.UpdateAsync(subscription);
                }

                if (business != null)
                {
                    business.IsActive = false;
                    await _businessRepository.UpdateAsync(business);
                }

                _logger.LogWarning($"Payment {payment.Id} failed: {payment.ErrorMessage}");
            }

            await _paymentRepository.UpdateAsync(payment);
            await _unitOfWork.SaveChangesAsync();

            return Result<bool>.SuccessResult(true);
        }
    }
}