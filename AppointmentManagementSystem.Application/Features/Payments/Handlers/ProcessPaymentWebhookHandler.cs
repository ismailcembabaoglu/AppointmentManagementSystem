using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
                _logger.LogInformation("=== Webhook Received ===");
                _logger.LogInformation($"MerchantOid: {request.MerchantOid}");
                _logger.LogInformation($"Status: {request.Status}");
                _logger.LogInformation($"TotalAmount: {request.TotalAmount}");
                _logger.LogInformation($"Utoken: {request.Utoken}");
                _logger.LogInformation($"Ctoken: {request.Ctoken}");
                _logger.LogInformation($"CardType: {request.CardType}");
                _logger.LogInformation($"MaskedPan: {request.MaskedPan}");
                _logger.LogInformation($"PaymentId: {request.PaymentId}");

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

                var merchantSalt = _configuration["PayTR:MerchantSalt"] ?? string.Empty;
                var expectedHash = _paytrService.ValidateWebhookSignature(
                    request.MerchantOid,
                    request.Status,
                    request.TotalAmount,
                    merchantSalt);

                _logger.LogInformation($"Expected Hash (Base64): {expectedHash}");
                _logger.LogInformation($"Received Hash: {request.Hash}");

                if (!string.Equals(expectedHash, request.Hash, StringComparison.Ordinal))
                {
                    _logger.LogError($"❌ Invalid webhook signature for MerchantOid: {request.MerchantOid}");
                    _logger.LogError($"Expected: {expectedHash}");
                    _logger.LogError($"Received: {request.Hash}");
                    return Result<bool>.FailureResult("Invalid webhook signature - security check failed");
                }

                if (request.MerchantOid.StartsWith("CARD", StringComparison.Ordinal) && request.Status == "success")
                {
                    _logger.LogInformation("✅ Processing card update callback");
                    return await HandleCardUpdateCallback(request, cancellationToken);
                }

                if (request.MerchantOid.StartsWith("BILL", StringComparison.Ordinal) && request.Status == "success")
                {
                    _logger.LogInformation("✅ Processing manual billing callback");
                    return await HandleManualBillingCallback(request, cancellationToken);
                }

                if (request.MerchantOid.StartsWith("REG", StringComparison.Ordinal) && request.Status == "success")
                {
                    _logger.LogInformation("✅ Processing initial registration callback");
                    return await HandleInitialRegistrationCallback(request, cancellationToken);
                }

                if (request.Status == "success")
                {
                    _logger.LogInformation("✅ Processing recurring payment callback");
                    return await HandlePaymentCallback(request, cancellationToken);
                }

                _logger.LogWarning($"⚠️ Payment failed or invalid status: {request.Status}");
                return Result<bool>.FailureResult($"Payment status: {request.Status}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ Error processing webhook for MerchantOid: {request.MerchantOid}");
                return Result<bool>.FailureResult($"Error processing webhook: {ex.Message}");
            }
        }

        private async Task<Result<bool>> HandleInitialRegistrationCallback(ProcessPaymentWebhookCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("=== HandleInitialRegistrationCallback Started ===");

            var merchantOid = request.MerchantOid;
            const string regPrefix = "REG";

            if (!merchantOid.StartsWith(regPrefix, StringComparison.Ordinal))
            {
                _logger.LogWarning($"Invalid MerchantOid format: {merchantOid}");
                return Result<bool>.FailureResult("Invalid MerchantOid format");
            }

            var afterReg = merchantOid.Substring(regPrefix.Length);
            string businessIdStr;

            if (afterReg.Contains("_", StringComparison.Ordinal))
            {
                var parts = afterReg.Split('_');
                businessIdStr = parts[0];
            }
            else
            {
                businessIdStr = new string(afterReg.TakeWhile(char.IsDigit).ToArray());
            }

            if (!int.TryParse(businessIdStr, out var businessId))
            {
                _logger.LogWarning($"Could not parse BusinessId from MerchantOid: {merchantOid}");
                return Result<bool>.FailureResult("Invalid MerchantOid format");
            }

            var business = await _businessRepository.GetByIdAsync(businessId);
            if (business == null)
            {
                _logger.LogWarning($"Business not found: {businessId}");
                return Result<bool>.FailureResult("Business not found");
            }

            var payment = new Payment
            {
                BusinessId = businessId,
                MerchantOid = request.MerchantOid,
                Amount = decimal.Parse(request.TotalAmount) / 100,
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

            var subscription = await _subscriptionRepository.GetByBusinessIdAsync(businessId);
            var hasCardTokens = !string.IsNullOrEmpty(request.Utoken) && !string.IsNullOrEmpty(request.Ctoken);

            if (!hasCardTokens)
            {
                _logger.LogWarning("⚠️ Card tokens (utoken/ctoken) are NULL. iFrame API does not support card tokenization.");
            }

            if (subscription == null)
            {
                subscription = new BusinessSubscription
                {
                    BusinessId = businessId,
                    PayTRUserToken = request.Utoken,
                    PayTRCardToken = request.Ctoken,
                    CardBrand = request.CardType,
                    CardType = request.CardType,
                    MaskedCardNumber = request.MaskedPan,
                    CardLastFourDigits = GetLastFour(request.MaskedPan),
                    MonthlyAmount = 700.00m,
                    Status = SubscriptionStatus.Active,
                    SubscriptionStatus = SubscriptionStatus.Active,
                    StartDate = DateTime.UtcNow,
                    SubscriptionStartDate = DateTime.UtcNow,
                    LastBillingDate = DateTime.UtcNow,
                    NextBillingDate = DateTime.UtcNow.AddDays(30),
                    IsActive = true,
                    AutoRenewal = hasCardTokens,
                    CreatedAt = DateTime.UtcNow
                };

                await _subscriptionRepository.AddAsync(subscription);
            }
            else
            {
                if (!string.IsNullOrEmpty(request.Utoken))
                {
                    subscription.PayTRUserToken = request.Utoken;
                }

                if (!string.IsNullOrEmpty(request.Ctoken))
                {
                    subscription.PayTRCardToken = request.Ctoken;
                }

                if (!string.IsNullOrEmpty(request.CardType))
                {
                    subscription.CardBrand = request.CardType;
                    subscription.CardType = request.CardType;
                }

                if (!string.IsNullOrEmpty(request.MaskedPan))
                {
                    subscription.MaskedCardNumber = request.MaskedPan;
                    subscription.CardLastFourDigits = GetLastFour(request.MaskedPan) ?? subscription.CardLastFourDigits;
                }

                subscription.Status = SubscriptionStatus.Active;
                subscription.SubscriptionStatus = SubscriptionStatus.Active;
                subscription.LastBillingDate = DateTime.UtcNow;
                subscription.NextBillingDate = DateTime.UtcNow.AddDays(30);
                subscription.IsActive = true;
                subscription.AutoRenewal = hasCardTokens || subscription.AutoRenewal;
                subscription.UpdatedAt = DateTime.UtcNow;

                await _subscriptionRepository.UpdateAsync(subscription);
            }

            business.IsActive = true;
            business.UpdatedAt = DateTime.UtcNow;
            await _businessRepository.UpdateAsync(business);

            await _unitOfWork.SaveChangesAsync();
            return Result<bool>.SuccessResult(true);
        }

        private async Task<Result<bool>> HandleManualBillingCallback(ProcessPaymentWebhookCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("=== HandleManualBillingCallback Started ===");

            const string billPrefix = "BILL";
            var merchantOid = request.MerchantOid;
            if (!merchantOid.StartsWith(billPrefix, StringComparison.Ordinal))
            {
                _logger.LogWarning($"Invalid MerchantOid format for manual billing: {merchantOid}");
                return Result<bool>.FailureResult("Invalid MerchantOid format");
            }

            var afterPrefix = merchantOid.Substring(billPrefix.Length);
            var businessIdStr = new string(afterPrefix.TakeWhile(char.IsDigit).ToArray());
            if (!int.TryParse(businessIdStr, out var businessId))
            {
                _logger.LogWarning($"Could not parse BusinessId from MerchantOid: {merchantOid}");
                return Result<bool>.FailureResult("Invalid MerchantOid format");
            }

            var targetPeriod = ExtractTargetPeriod(afterPrefix);
            var business = await _businessRepository.GetByIdAsync(businessId);
            if (business == null)
            {
                _logger.LogWarning($"Business not found: {businessId}");
                return Result<bool>.FailureResult("Business not found");
            }

            var amount = decimal.TryParse(request.TotalAmount, out var parsedAmount) ? parsedAmount / 100 : 0m;
            var subscription = await _subscriptionRepository.GetByBusinessIdAsync(businessId);
            if (subscription == null)
            {
                subscription = new BusinessSubscription
                {
                    BusinessId = businessId,
                    MonthlyAmount = amount,
                    Currency = "TRY",
                    Status = SubscriptionStatus.Active,
                    SubscriptionStatus = SubscriptionStatus.Active,
                    StartDate = targetPeriod,
                    SubscriptionStartDate = targetPeriod,
                    LastBillingDate = targetPeriod,
                    NextBillingDate = targetPeriod.AddMonths(1),
                    AutoRenewal = false,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                await _subscriptionRepository.AddAsync(subscription);
            }
            else
            {
                subscription.LastBillingDate = targetPeriod;
                subscription.NextBillingDate = targetPeriod.AddMonths(1);
                subscription.SubscriptionStatus = SubscriptionStatus.Active;
                subscription.Status = SubscriptionStatus.Active;
                subscription.IsActive = true;
                subscription.UpdatedAt = DateTime.UtcNow;

                if (!string.IsNullOrEmpty(request.Utoken))
                {
                    subscription.PayTRUserToken = request.Utoken;
                }

                if (!string.IsNullOrEmpty(request.Ctoken))
                {
                    subscription.PayTRCardToken = request.Ctoken;
                }

                if (!string.IsNullOrEmpty(request.MaskedPan))
                {
                    subscription.MaskedCardNumber = request.MaskedPan;
                    subscription.CardLastFourDigits = GetLastFour(request.MaskedPan) ?? subscription.CardLastFourDigits;
                }

                if (!string.IsNullOrEmpty(request.CardType))
                {
                    subscription.CardBrand = request.CardType;
                    subscription.CardType = request.CardType;
                }

                await _subscriptionRepository.UpdateAsync(subscription);
            }

            var payment = new Payment
            {
                BusinessId = businessId,
                MerchantOid = request.MerchantOid,
                Amount = amount,
                Currency = "TRY",
                Status = request.Status,
                PaymentDate = DateTime.UtcNow,
                CardType = request.CardType,
                MaskedCardNumber = request.MaskedPan,
                PaymentType = "ManualBilling",
                PayTRTransactionId = request.PaymentId,
                CreatedAt = DateTime.UtcNow
            };

            await _paymentRepository.AddAsync(payment);

            business.IsActive = true;
            business.UpdatedAt = DateTime.UtcNow;
            await _businessRepository.UpdateAsync(business);

            await _unitOfWork.SaveChangesAsync();
            return Result<bool>.SuccessResult(true);
        }

        private async Task<Result<bool>> HandleCardUpdateCallback(ProcessPaymentWebhookCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("=== HandleCardUpdateCallback Started ===");

            const string prefix = "CARD";
            var merchantOid = request.MerchantOid;
            if (!merchantOid.StartsWith(prefix, StringComparison.Ordinal))
            {
                _logger.LogWarning($"Invalid CARD MerchantOid format: {merchantOid}");
                return Result<bool>.FailureResult("Invalid MerchantOid format");
            }

            var afterPrefix = merchantOid.Substring(prefix.Length);
            var businessIdStr = afterPrefix.Contains("_", StringComparison.Ordinal)
                ? afterPrefix.Split('_')[0]
                : new string(afterPrefix.TakeWhile(char.IsDigit).ToArray());

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
                subscription = new BusinessSubscription
                {
                    BusinessId = businessId,
                    PayTRUserToken = request.Utoken,
                    PayTRCardToken = request.Ctoken,
                    CardBrand = request.CardType,
                    CardType = request.CardType,
                    MaskedCardNumber = request.MaskedPan,
                    CardLastFourDigits = GetLastFour(request.MaskedPan),
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
                subscription.CardLastFourDigits = GetLastFour(request.MaskedPan) ?? subscription.CardLastFourDigits;
                subscription.UpdatedAt = DateTime.UtcNow;

                await _subscriptionRepository.UpdateAsync(subscription);
            }

            var amount = decimal.TryParse(request.TotalAmount, out var parsedAmount) ? parsedAmount / 100 : 0m;
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
                return Result<bool>.SuccessResult(true);
            }

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

        private static string? GetLastFour(string? maskedPan)
        {
            if (string.IsNullOrEmpty(maskedPan) || maskedPan.Length < 4)
            {
                return null;
            }

            return maskedPan.Substring(maskedPan.Length - 4);
        }

        private static DateTime ExtractTargetPeriod(string afterPrefix)
        {
            var mIndex = afterPrefix.IndexOf('M');
            if (mIndex >= 0 && afterPrefix.Length >= mIndex + 7)
            {
                var yearSegment = afterPrefix.Substring(mIndex + 1, 4);
                var monthSegment = afterPrefix.Substring(mIndex + 5, 2);

                if (int.TryParse(yearSegment, out var billingYear) && int.TryParse(monthSegment, out var billingMonth) && billingMonth >= 1 && billingMonth <= 12)
                {
                    return new DateTime(billingYear, billingMonth, 1, 0, 0, 0, DateTimeKind.Utc);
                }
            }

            return DateTime.UtcNow;
        }
    }
}
