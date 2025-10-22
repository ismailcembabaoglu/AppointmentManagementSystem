using MediatR;

namespace AppointmentManagementSystem.Application.Features.Appointments.Commands
{
    public class DeleteAppointmentCommand : IRequest<bool>
    {
        public int Id { get; set; }
    }
}
