using AppointmentManagementSystem.Application.DTOs;
using MediatR;

namespace AppointmentManagementSystem.Application.Features.Services.Commands
{
    public class UpdateServiceCommand : IRequest<ServiceDto?>
    {
        public int Id { get; set; }
        public CreateServiceDto CreateServiceDto { get; set; } = new();
    }
}
