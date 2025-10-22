using AppointmentManagementSystem.Application.DTOs;
using MediatR;

namespace AppointmentManagementSystem.Application.Features.Appointments.Commands
{
    public class CreateAppointmentCommand : IRequest<AppointmentDto>
    {
        public CreateAppointmentDto CreateAppointmentDto { get; set; } = new();
    }
}
