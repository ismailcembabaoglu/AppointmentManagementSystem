using AppointmentManagementSystem.Application.DTOs;
using AppointmentManagementSystem.Application.Shared;
using MediatR;

namespace AppointmentManagementSystem.Application.Features.Payments.Queries
{
    public class GetSubscriptionByBusinessIdQuery : IRequest<Result<SubscriptionDto>>
    {
        public int BusinessId { get; set; }
    }

    public class GetPaymentHistoryQuery : IRequest<Result<List<PaymentDto>>>
    {
        public int BusinessId { get; set; }
    }
}