using AppointmentManagementSystem.Application.DTOs;
using MediatR;

namespace AppointmentManagementSystem.Application.Features.Businesses.Queries
{
    public class GetServicesByBusinessQuery : IRequest<List<ServiceDto>>
    {
        public int BusinessId { get; set; }
    }
}
