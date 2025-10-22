using AppointmentManagementSystem.Application.DTOs;
using MediatR;

namespace AppointmentManagementSystem.Application.Features.Appointments.Commands
{
    public class UpdateAppointmentStatusCommand : IRequest<AppointmentDto?>
    {
        public int Id { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
