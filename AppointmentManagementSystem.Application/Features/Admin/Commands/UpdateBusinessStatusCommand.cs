using MediatR;

namespace AppointmentManagementSystem.Application.Features.Admin.Commands
{
    public class UpdateBusinessStatusCommand : IRequest<bool>
    {
        public int BusinessId { get; set; }
        public bool IsActive { get; set; }
    }
}