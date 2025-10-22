using AppointmentManagementSystem.Application.DTOs;
using MediatR;

namespace AppointmentManagementSystem.Application.Features.Services.Queries
{
    public class GetServiceByIdQuery : IRequest<ServiceDto?>
    {
        public int Id { get; set; }
    }
}
