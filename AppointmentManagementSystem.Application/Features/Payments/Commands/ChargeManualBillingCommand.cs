using AppointmentManagementSystem.Application.Shared;
using MediatR;

namespace AppointmentManagementSystem.Application.Features.Payments.Commands
{
    public class ChargeManualBillingCommand : IRequest<Result<ChargeManualBillingResponse>>
    {
        public int BusinessId { get; set; }
        public int BillingYear { get; set; }
        public int BillingMonth { get; set; }
        public string UserIp { get; set; } = string.Empty;
    }

    public class ChargeManualBillingResponse
    {
        public bool Success { get; set; }
        public string? MerchantOid { get; set; }
        public string? Message { get; set; }
    }
}
