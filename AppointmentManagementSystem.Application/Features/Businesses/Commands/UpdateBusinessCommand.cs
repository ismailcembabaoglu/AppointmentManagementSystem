using AppointmentManagementSystem.Application.DTOs;
using MediatR;

namespace AppointmentManagementSystem.Application.Features.Businesses.Commands
{
    public class UpdateBusinessCommand : IRequest<BusinessDto?>
    {
        public int Id { get; set; }
        public CreateBusinessDto CreateBusinessDto { get; set; } = new();
    }
}
