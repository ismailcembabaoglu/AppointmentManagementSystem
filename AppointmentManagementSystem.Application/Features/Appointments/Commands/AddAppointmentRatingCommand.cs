using AppointmentManagementSystem.Application.DTOs;
using MediatR;

namespace AppointmentManagementSystem.Application.Features.Appointments.Commands
{
    public class AddAppointmentRatingCommand : IRequest<AppointmentDto?>
    {
        public int Id { get; set; }
        public int Rating { get; set; }
        public string? Review { get; set; }
    }
}
