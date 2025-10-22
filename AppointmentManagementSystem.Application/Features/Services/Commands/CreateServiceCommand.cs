using AppointmentManagementSystem.Application.DTOs;
using MediatR;

namespace AppointmentManagementSystem.Application.Features.Services.Commands
{
    public class CreateServiceCommand : IRequest<ServiceDto>
    {
        public CreateServiceDto CreateServiceDto { get; set; } = new();
    }
}
