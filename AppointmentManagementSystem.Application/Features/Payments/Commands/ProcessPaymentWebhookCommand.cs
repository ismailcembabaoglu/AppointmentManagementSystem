using AppointmentManagementSystem.Application.Shared;
using MediatR;

namespace AppointmentManagementSystem.Application.Features.Payments.Commands
{
    public class ProcessPaymentWebhookCommand : IRequest<Result<bool>>
    {
        public string MerchantOid { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string TotalAmount { get; set; } = string.Empty;
        public string Hash { get; set; } = string.Empty;
        public string? Utoken { get; set; }
        public string? Ctoken { get; set; }
        public string? CardType { get; set; }
        public string? MaskedPan { get; set; }
        public string? PaymentId { get; set; }
        public string? FailedReasonMsg { get; set; }
    }
}