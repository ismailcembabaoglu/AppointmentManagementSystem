using MediatR;

namespace AppointmentManagementSystem.Application.Features.Admin.Commands
{
    public class DeleteAppointmentAdminCommand : IRequest<bool>
    {
        public int AppointmentId { get; set; }
    }
}