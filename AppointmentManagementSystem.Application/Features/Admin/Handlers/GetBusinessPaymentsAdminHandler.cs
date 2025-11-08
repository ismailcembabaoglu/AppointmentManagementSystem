using AppointmentManagementSystem.Application.Features.Admin.Queries;
using AppointmentManagementSystem.Domain.Interfaces;
using MediatR;

namespace AppointmentManagementSystem.Application.Features.Admin.Handlers
{
    public class GetBusinessPaymentsAdminHandler : IRequestHandler<GetBusinessPaymentsAdminQuery, List<PaymentAdminDto>>
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IBusinessRepository _businessRepository;

        public GetBusinessPaymentsAdminHandler(
            IPaymentRepository paymentRepository,
            IBusinessRepository businessRepository)
        {
            _paymentRepository = paymentRepository;
            _businessRepository = businessRepository;
        }

        public async Task<List<PaymentAdminDto>> Handle(GetBusinessPaymentsAdminQuery request, CancellationToken cancellationToken)
        {
            var payments = await _paymentRepository.GetByBusinessIdAsync(request.BusinessId);
            var business = await _businessRepository.GetByIdAsync(request.BusinessId);

            var query = payments.AsEnumerable();

            if (request.StartDate.HasValue)
            {
                query = query.Where(p => p.PaymentDate >= request.StartDate.Value);
            }

            if (request.EndDate.HasValue)
            {
                query = query.Where(p => p.PaymentDate <= request.EndDate.Value);
            }

            return query.Select(p => new PaymentAdminDto
            {
                Id = p.Id,
                BusinessId = p.BusinessId,
                BusinessName = business?.Name ?? "N/A",
                Amount = p.Amount,
                Status = p.Status,
                TransactionId = p.TransactionId,
                PaymentDate = p.PaymentDate,
                CardType = p.CardType,
                MaskedCardNumber = p.MaskedCardNumber,
                PaymentType = p.PaymentType,
                RetryAttempt = p.RetryAttempt,
                ErrorMessage = p.ErrorMessage,
                CreatedAt = p.CreatedAt
            }).OrderByDescending(p => p.PaymentDate).ToList();
        }
    }
}