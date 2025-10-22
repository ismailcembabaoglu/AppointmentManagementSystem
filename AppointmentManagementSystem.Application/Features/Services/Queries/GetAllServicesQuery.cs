using AppointmentManagementSystem.Application.DTOs;
using MediatR;

namespace AppointmentManagementSystem.Application.Features.Services.Queries
{
    public class GetAllServicesQuery : IRequest<List<ServiceDto>>
    {
        public int? BusinessId { get; set; }
    }
}
