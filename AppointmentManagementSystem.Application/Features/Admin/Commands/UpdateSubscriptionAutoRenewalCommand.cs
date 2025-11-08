using MediatR;

namespace AppointmentManagementSystem.Application.Features.Admin.Commands
{
    public class UpdateSubscriptionAutoRenewalCommand : IRequest<bool>
    {
        public int BusinessId { get; set; }
        public bool AutoRenewal { get; set; }
    }
}