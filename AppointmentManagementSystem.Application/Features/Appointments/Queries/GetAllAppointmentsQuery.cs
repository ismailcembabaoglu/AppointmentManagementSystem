using AppointmentManagementSystem.Application.DTOs;
using MediatR;

namespace AppointmentManagementSystem.Application.Features.Appointments.Queries
{
    public class GetAllAppointmentsQuery : IRequest<List<AppointmentDto>>
    {
        public int? CustomerId { get; set; }
        public int? BusinessId { get; set; }
    }
}
