using AppointmentManagementSystem.Application.DTOs;
using MediatR;

namespace AppointmentManagementSystem.Application.Features.Appointments.Queries
{
    public class GetAppointmentByIdQuery : IRequest<AppointmentDto?>
    {
        public int Id { get; set; }
    }
}
