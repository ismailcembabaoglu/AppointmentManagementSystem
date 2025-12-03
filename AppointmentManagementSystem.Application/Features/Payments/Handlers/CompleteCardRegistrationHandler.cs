using AppointmentManagementSystem.Application.Features.Payments.Commands;
using AppointmentManagementSystem.Application.Shared;
using AppointmentManagementSystem.Domain.Entities;
using AppointmentManagementSystem.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AppointmentManagementSystem.Application.Features.Payments.Handlers
{
    public class CompleteCardRegistrationHandler : IRequestHandler<CompleteCardRegistrationCommand, Result<bool>>
    {
        private readonly IBusinessRepository _businessRepository;
        private readonly IBusinessSubscriptionRepository _subscriptionRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CompleteCardRegistrationHandler> _logger;

        public CompleteCardRegistrationHandler(
            IBusinessRepository businessRepository,
            IBusinessSubscriptionRepository subscriptionRepository,
            IPaymentRepository paymentRepository,
            IUnitOfWork unitOfWork,
            ILogger<CompleteCardRegistrationHandler> logger)
        {
            _businessRepository = businessRepository;
            _subscriptionRepository = subscriptionRepository;
            _paymentRepository = paymentRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result<bool>> Handle(CompleteCardRegistrationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (request.BusinessId <= 0)
                {
                    return Result<bool>.FailureResult("BusinessId is required");
                }

                var business = await _businessRepository.GetByIdAsync(request.BusinessId);
                if (business == null)
                {
                    _logger.LogWarning($"Business not found for card registration: {request.BusinessId}");
                    return Result<bool>.FailureResult("Business not found");
                }

                var subscription = await _subscriptionRepository.GetByBusinessIdAsync(request.BusinessId);
                var isCardUpdate = request.IsCardUpdate || request.MerchantOid.StartsWith("CARD");

                if (subscription == null)
                {
                    subscription = new BusinessSubscription
                    {
                        BusinessId = request.BusinessId,
                        PayTRUserToken = request.Utoken,
                        PayTRCardToken = request.Ctoken,
                        CardBrand = request.CardType,
                        CardType = request.CardType,
                        MaskedCardNumber = request.MaskedPan,
                        CardLastFourDigits = request.MaskedPan != null && request.MaskedPan.Length >= 4
                            ? request.MaskedPan.Substring(request.MaskedPan.Length - 4)
                            : null,
                        MonthlyAmount = 700m,
                        Status = SubscriptionStatus.Active,
                        SubscriptionStatus = SubscriptionStatus.Active,
                        StartDate = DateTime.Now,
                        SubscriptionStartDate = DateTime.Now,
                        AutoRenewal = true,
                        IsActive = true,
                        CreatedAt = DateTime.Now,
                        NextBillingDate = DateTime.Now.AddDays(30)
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
                    subscription.UpdatedAt = DateTime.Now;

                    await _subscriptionRepository.UpdateAsync(subscription);
                }

                var amount = decimal.TryParse(request.TotalAmount, out var parsedAmount)
                    ? parsedAmount / 100
                    : (isCardUpdate ? 1m : 700m);

                var payment = new Payment
                {
                    BusinessId = request.BusinessId,
                    MerchantOid = request.MerchantOid,
                    Amount = amount,
                    Currency = "TRY",
                    Status = PaymentStatus.Success,
                    PaymentDate = DateTime.Now,
                    CardType = request.CardType,
                    MaskedCardNumber = request.MaskedPan,
                    PaymentType = isCardUpdate ? "CardUpdate" : "ManualWebhook",
                    PayTRTransactionId = request.PaymentId,
                    CreatedAt = DateTime.Now
                };

                await _paymentRepository.AddAsync(payment);

                business.IsActive = true;
                business.UpdatedAt = DateTime.Now;
                await _businessRepository.UpdateAsync(business);

                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation($"Card registration completed for Business {request.BusinessId}. Update: {isCardUpdate}");
                return Result<bool>.SuccessResult(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error completing card registration");
                return Result<bool>.FailureResult(ex.Message);
            }
        }
    }
}
