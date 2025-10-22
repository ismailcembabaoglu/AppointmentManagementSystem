using AppointmentManagementSystem.Application.DTOs;
using MediatR;

namespace AppointmentManagementSystem.Application.Features.Businesses.Queries
{
    public class GetAppointmentsByBusinessQuery : IRequest<List<AppointmentDto>>
    {
        public int BusinessId { get; set; }
    }
}
