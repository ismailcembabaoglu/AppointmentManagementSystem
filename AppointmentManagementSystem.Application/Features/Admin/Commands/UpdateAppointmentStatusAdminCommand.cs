using MediatR;

namespace AppointmentManagementSystem.Application.Features.Admin.Commands
{
    public class UpdateAppointmentStatusAdminCommand : IRequest<bool>
    {
        public int AppointmentId { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}