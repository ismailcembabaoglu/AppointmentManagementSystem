using AppointmentManagementSystem.Application.DTOs;
using AppointmentManagementSystem.Application.Features.Payments.Queries;
using AppointmentManagementSystem.Application.Shared;
using AppointmentManagementSystem.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AppointmentManagementSystem.Application.Features.Payments.Handlers
{
    public class GetSubscriptionByBusinessIdHandler : IRequestHandler<GetSubscriptionByBusinessIdQuery, Result<SubscriptionDto>>
    {
        private readonly IBusinessSubscriptionRepository _subscriptionRepository;
        private readonly ILogger<GetSubscriptionByBusinessIdHandler> _logger;

        public GetSubscriptionByBusinessIdHandler(
            IBusinessSubscriptionRepository subscriptionRepository,
            ILogger<GetSubscriptionByBusinessIdHandler> logger)
        {
            _subscriptionRepository = subscriptionRepository;
            _logger = logger;
        }

        public async Task<Result<SubscriptionDto>> Handle(GetSubscriptionByBusinessIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var subscription = await _subscriptionRepository.GetByBusinessIdAsync(request.BusinessId);
                
                if (subscription == null)
                {
                    return Result<SubscriptionDto>.FailureResult("Subscription not found");
                }

                var dto = new SubscriptionDto
                {
                    Id = subscription.Id,
                    BusinessId = subscription.BusinessId,
                    BusinessName = subscription.Business?.Name ?? "",
                    MonthlyAmount = subscription.MonthlyAmount,
                    Currency = subscription.Currency,
                    SubscriptionStatus = subscription.SubscriptionStatus,
                    NextBillingDate = subscription.NextBillingDate,
                    LastBillingDate = subscription.LastBillingDate,
                    IsActive = subscription.IsActive,
                    CardBrand = subscription.CardBrand,
                    MaskedCardNumber = subscription.MaskedCardNumber
                };

                return Result<SubscriptionDto>.SuccessResult(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting subscription for Business {request.BusinessId}");
                return Result<SubscriptionDto>.FailureResult("Error retrieving subscription");
            }
        }
    }

    public class GetPaymentHistoryHandler : IRequestHandler<GetPaymentHistoryQuery, Result<List<PaymentDto>>>
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly ILogger<GetPaymentHistoryHandler> _logger;

        public GetPaymentHistoryHandler(
            IPaymentRepository paymentRepository,
            ILogger<GetPaymentHistoryHandler> logger)
        {
            _paymentRepository = paymentRepository;
            _logger = logger;
        }

        public async Task<Result<List<PaymentDto>>> Handle(GetPaymentHistoryQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var payments = await _paymentRepository.GetByBusinessIdAsync(request.BusinessId);

                var dtos = payments.Select(p => new PaymentDto
                {
                    Id = p.Id,
                    BusinessId = p.BusinessId,
                    MerchantOid = p.MerchantOid,
                    Amount = p.Amount,
                    Currency = p.Currency,
                    Status = p.Status,
                    PaymentDate = p.PaymentDate,
                    RetryCount = p.RetryCount,
                    ErrorMessage = p.ErrorMessage,
                    CreatedAt = p.CreatedAt
                }).ToList();

                return Result<List<PaymentDto>>.SuccessResult(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting payment history for Business {request.BusinessId}");
                return Result<List<PaymentDto>>.FailureResult("Error retrieving payment history");
            }
        }
    }
}
