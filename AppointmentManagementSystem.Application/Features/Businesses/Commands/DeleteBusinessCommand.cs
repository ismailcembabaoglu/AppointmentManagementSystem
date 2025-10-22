using MediatR;

namespace AppointmentManagementSystem.Application.Features.Businesses.Commands
{
    public class DeleteBusinessCommand : IRequest<bool>
    {
        public int Id { get; set; }
    }
}
