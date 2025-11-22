using AppointmentManagementSystem.Application.Features.Admin.Commands;
using AppointmentManagementSystem.Domain.Interfaces;
using MediatR;

namespace AppointmentManagementSystem.Application.Features.Admin.Handlers
{
    public class RefundPaymentHandler : IRequestHandler<RefundPaymentCommand, bool>
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RefundPaymentHandler(
            IPaymentRepository paymentRepository,
            IUnitOfWork unitOfWork)
        {
            _paymentRepository = paymentRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(RefundPaymentCommand request, CancellationToken cancellationToken)
        {
            var payment = await _paymentRepository.GetByIdAsync(request.PaymentId);
            if (payment == null)
                return false;

            payment.Status = "Refunded";
            payment.ErrorMessage = request.Reason;
            payment.UpdatedAt = DateTime.UtcNow;

            await _paymentRepository.UpdateAsync(payment);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}