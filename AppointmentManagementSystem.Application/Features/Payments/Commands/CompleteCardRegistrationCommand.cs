using AppointmentManagementSystem.Application.DTOs;
using AppointmentManagementSystem.Application.Shared;
using MediatR;

namespace AppointmentManagementSystem.Application.Features.Payments.Commands
{
    public class CompleteCardRegistrationCommand : IRequest<Result<bool>>
    {
        public int BusinessId { get; set; }
        public string MerchantOid { get; set; } = string.Empty;
        public string? Utoken { get; set; }
        public string? Ctoken { get; set; }
        public string? CardType { get; set; }
        public string? MaskedPan { get; set; }
        public string? PaymentId { get; set; }
        public string? TotalAmount { get; set; }
        public bool IsCardUpdate { get; set; }
    }
}
