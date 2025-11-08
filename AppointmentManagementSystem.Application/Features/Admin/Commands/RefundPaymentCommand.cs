using MediatR;

namespace AppointmentManagementSystem.Application.Features.Admin.Commands
{
    public class RefundPaymentCommand : IRequest<bool>
    {
        public int PaymentId { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}