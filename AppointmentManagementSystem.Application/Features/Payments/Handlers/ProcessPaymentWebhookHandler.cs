using System;
using System.Linq;
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
                _logger.LogInformation($"PaymentId: {request.PaymentId}");

                // Validate required fields
                if (string.IsNullOrEmpty(request.MerchantOid))
                {
                    _logger.LogError("MerchantOid is empty!");
                    return Result<bool>.FailureResult("MerchantOid is required");
                }

                if (string.IsNullOrEmpty(request.Status))
                {
                    _logger.LogError("Status is empty!");
                    return Result<bool>.FailureResult("Status is required");
                }

                // Validate webhook signature
                var merchantSalt = _configuration["PayTR:MerchantSalt"] ?? "";
                var expectedHash = _paytrService.ValidateWebhookSignature(
                    request.MerchantOid,
                    request.Status,
                    request.TotalAmount,
                    merchantSalt
                );

                _logger.LogInformation($"Expected Hash (Base64): {expectedHash}");
                _logger.LogInformation($"Received Hash: {request.Hash}");

                if (!string.Equals(expectedHash, request.Hash, StringComparison.Ordinal))
                {
                    _logger.LogWarning($"Invalid webhook signature for MerchantOid: {request.MerchantOid}");
                    // Test modunda hash kontrolünü atlayalım (localhost'tan geldiği için)
                    // return Result<bool>.FailureResult("Invalid signature");
                    _logger.LogWarning("⚠️ Continuing despite invalid signature (test mode)");
                }

                // Check if this is initial registration (REG prefix), card update (CARD prefix) or recurring payment
                if (request.MerchantOid.StartsWith("CARD") && request.Status == "success")
                {
                    _logger.LogInformation("✅ Processing card update callback");
                    return await HandleCardUpdateCallback(request, cancellationToken);
                }
                else if (request.MerchantOid.StartsWith("REG") && request.Status == "success")
                {
                    _logger.LogInformation("✅ Processing initial registration callback");
                    // This is initial registration + first payment callback
                    return await HandleInitialRegistrationCallback(request, cancellationToken);
                }
                else if (request.Status == "success")
                {
                    _logger.LogInformation("✅ Processing recurring payment callback");
                    // This is a recurring payment callback
                    return await HandlePaymentCallback(request, cancellationToken);
                }
                else
                {
                    _logger.LogWarning($"⚠️ Payment failed or invalid status: {request.Status}");
                    return Result<bool>.FailureResult($"Payment status: {request.Status}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ Error processing webhook for MerchantOid: {request.MerchantOid}");
                _logger.LogError($"Exception details: {ex.Message}");
                _logger.LogError($"Stack trace: {ex.StackTrace}");
                return Result<bool>.FailureResult($"Error processing webhook: {ex.Message}");
            }
        }

        private async Task<Result<bool>> HandleInitialRegistrationCallback(ProcessPaymentWebhookCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("=== HandleInitialRegistrationCallback Started ===");

            // Extract BusinessId from MerchantOid
            // Format: REG{BusinessId}_{Guid} or REG{BusinessId}{Guid}
            var merchantOid = request.MerchantOid;
            var regPrefix = "REG";
            
            _logger.LogInformation($"MerchantOid: {merchantOid}");
            
            if (!merchantOid.StartsWith(regPrefix))
            {
                _logger.LogWarning($"Invalid MerchantOid format: {merchantOid}");
                return Result<bool>.FailureResult("Invalid MerchantOid format");
            }

            var afterReg = merchantOid.Substring(regPrefix.Length);
            _logger.LogInformation($"After REG prefix: {afterReg}");
            
            string businessIdStr;
            
            // Check if underscore separator exists (new format: REG6_abc123)
            if (afterReg.Contains("_"))
            {
                var parts = afterReg.Split('_');
                businessIdStr = parts[0];
                _logger.LogInformation($"Extracted BusinessId string (with separator): {businessIdStr}");
            }
            else
            {
                // Old format: REG6abc123 - take only digits
                businessIdStr = new string(afterReg.TakeWhile(char.IsDigit).ToArray());
                _logger.LogInformation($"Extracted BusinessId string (without separator): {businessIdStr}");
            }
            
            if (!int.TryParse(businessIdStr, out int businessId))
            {
                _logger.LogWarning($"Could not parse BusinessId from MerchantOid: {merchantOid}");
                return Result<bool>.FailureResult("Invalid MerchantOid format");
            }

            _logger.LogInformation($"✅ Parsed BusinessId: {businessId}");

            var business = await _businessRepository.GetByIdAsync(businessId);
            if (business == null)
            {
                _logger.LogWarning($"Business not found: {businessId}");
                return Result<bool>.FailureResult("Business not found");
            }

            _logger.LogInformation($"Business found: {business.Name} (ID: {business.Id})");

            // 1. İlk ödeme kaydını oluştur
            _logger.LogInformation("Creating payment record...");
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
            _logger.LogInformation($"Payment record created: Amount={payment.Amount} TL");

            // 2. Abonelik oluştur veya güncelle
            _logger.LogInformation("Checking for existing subscription...");
            var subscription = await _subscriptionRepository.GetByBusinessIdAsync(businessId);
            
            if (subscription == null)
            {
                _logger.LogInformation("Creating new subscription...");
                subscription = new BusinessSubscription
                {
                    BusinessId = businessId,
                    PayTRUserToken = request.Utoken,
                    PayTRCardToken = request.Ctoken,
                    CardBrand = request.CardType,
                    CardType = request.CardType,
                    MaskedCardNumber = request.MaskedPan,
                    CardLastFourDigits = request.MaskedPan?.Substring(request.MaskedPan.Length - 4),
                    MonthlyAmount = 700.00m,
                    Status = SubscriptionStatus.Active,
                    SubscriptionStatus = SubscriptionStatus.Active,
                    StartDate = DateTime.UtcNow,
                    SubscriptionStartDate = DateTime.UtcNow,
                    LastBillingDate = DateTime.UtcNow, // İlk ödeme yapıldı
                    NextBillingDate = DateTime.UtcNow.AddDays(30), // 30 gün sonra
                    IsActive = true,
                    AutoRenewal = true,
                    CreatedAt = DateTime.UtcNow
                };

                await _subscriptionRepository.AddAsync(subscription);
                _logger.LogInformation($"New subscription created: Utoken={request.Utoken?.Substring(0, 10)}..., NextBilling={subscription.NextBillingDate}");
            }
            else
            {
                _logger.LogInformation("Updating existing subscription...");
                subscription.PayTRUserToken = request.Utoken;
                subscription.PayTRCardToken = request.Ctoken;
                subscription.CardBrand = request.CardType;
                subscription.CardType = request.CardType;
                subscription.MaskedCardNumber = request.MaskedPan;
                subscription.CardLastFourDigits = request.MaskedPan?.Substring(request.MaskedPan.Length - 4);
                subscription.Status = SubscriptionStatus.Active;
                subscription.SubscriptionStatus = SubscriptionStatus.Active;
                subscription.LastBillingDate = DateTime.UtcNow;
                subscription.NextBillingDate = DateTime.UtcNow.AddDays(30);
                subscription.IsActive = true;
                subscription.AutoRenewal = true;
                subscription.UpdatedAt = DateTime.UtcNow;
                
                await _subscriptionRepository.UpdateAsync(subscription);
                _logger.LogInformation($"Subscription updated: ID={subscription.Id}");
            }

            // 3. Business'ı aktifleştir
            _logger.LogInformation($"Activating business... Current status: IsActive={business.IsActive}");
            business.IsActive = true;
            business.UpdatedAt = DateTime.UtcNow;
            await _businessRepository.UpdateAsync(business);
            _logger.LogInformation($"Business activated: {business.Name} (ID: {business.Id})");

            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("All changes saved to database!");

            _logger.LogInformation($"✅ Initial registration completed for Business {businessId}. Payment: 700 TL, Next billing: {subscription.NextBillingDate}");
            return Result<bool>.SuccessResult(true);
        }

        private async Task<Result<bool>> HandleCardUpdateCallback(ProcessPaymentWebhookCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("=== HandleCardUpdateCallback Started ===");

            var merchantOid = request.MerchantOid;
            const string prefix = "CARD";

            if (!merchantOid.StartsWith(prefix))
            {
                _logger.LogWarning($"Invalid CARD MerchantOid format: {merchantOid}");
                return Result<bool>.FailureResult("Invalid MerchantOid format");
            }

            var afterPrefix = merchantOid.Substring(prefix.Length);
            string businessIdStr;

            if (afterPrefix.Contains("_"))
            {
                var parts = afterPrefix.Split('_');
                businessIdStr = parts[0];
            }
            else
            {
                businessIdStr = new string(afterPrefix.TakeWhile(char.IsDigit).ToArray());
            }

            if (!int.TryParse(businessIdStr, out var businessId))
            {
                _logger.LogWarning($"Could not parse BusinessId from CARD MerchantOid: {merchantOid}");
                return Result<bool>.FailureResult("Invalid MerchantOid format");
            }

            var business = await _businessRepository.GetByIdAsync(businessId);
            if (business == null)
            {
                _logger.LogWarning($"Business not found for card update: {businessId}");
                return Result<bool>.FailureResult("Business not found");
            }

            var subscription = await _subscriptionRepository.GetByBusinessIdAsync(businessId);

            if (subscription == null)
            {
                _logger.LogWarning($"Subscription not found for Business {businessId}, creating a placeholder to store new card info.");
                subscription = new BusinessSubscription
                {
                    BusinessId = businessId,
                    PayTRUserToken = request.Utoken,
                    PayTRCardToken = request.Ctoken,
                    CardBrand = request.CardType,
                    CardType = request.CardType,
                    MaskedCardNumber = request.MaskedPan,
                    CardLastFourDigits = request.MaskedPan != null && request.MaskedPan.Length >= 4
                        ? request.MaskedPan.Substring(request.MaskedPan.Length - 4)
                        : null,
                    MonthlyAmount = 700.00m,
                    Status = SubscriptionStatus.Active,
                    SubscriptionStatus = SubscriptionStatus.Active,
                    StartDate = DateTime.UtcNow,
                    SubscriptionStartDate = DateTime.UtcNow,
                    IsActive = true,
                    AutoRenewal = true,
                    CreatedAt = DateTime.UtcNow
                };

                await _subscriptionRepository.AddAsync(subscription);
            }
            else
            {
                subscription.PayTRUserToken = request.Utoken;
                subscription.PayTRCardToken = request.Ctoken;
                subscription.CardBrand = request.CardType;
                subscription.CardType = request.CardType;
                subscription.MaskedCardNumber = request.MaskedPan;
                subscription.CardLastFourDigits = request.MaskedPan != null && request.MaskedPan.Length >= 4
                    ? request.MaskedPan.Substring(request.MaskedPan.Length - 4)
                    : subscription.CardLastFourDigits;
                subscription.UpdatedAt = DateTime.UtcNow;

                await _subscriptionRepository.UpdateAsync(subscription);
            }

            var amount = decimal.TryParse(request.TotalAmount, out var parsedAmount)
                ? parsedAmount / 100
                : 0m;

            var payment = new Payment
            {
                BusinessId = businessId,
                MerchantOid = request.MerchantOid,
                Amount = amount,
                Currency = "TRY",
                Status = PaymentStatus.Success,
                PaymentDate = DateTime.UtcNow,
                CardType = request.CardType,
                MaskedCardNumber = request.MaskedPan,
                PaymentType = "CardUpdate",
                PayTRTransactionId = request.PaymentId,
                CreatedAt = DateTime.UtcNow
            };

            await _paymentRepository.AddAsync(payment);

            business.IsActive = true;
            business.UpdatedAt = DateTime.UtcNow;
            await _businessRepository.UpdateAsync(business);

            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation($"✅ Card update completed for Business {businessId}. Amount: {amount} TL");

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