using MediatR;

namespace AppointmentManagementSystem.Application.Features.Services.Commands
{
    public class DeleteServiceCommand : IRequest<bool>
    {
        public int Id { get; set; }
    }
}
