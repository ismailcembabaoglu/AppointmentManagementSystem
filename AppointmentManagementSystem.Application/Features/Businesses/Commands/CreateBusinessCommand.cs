using AppointmentManagementSystem.Application.DTOs;
using MediatR;

namespace AppointmentManagementSystem.Application.Features.Businesses.Commands
{
    public class CreateBusinessCommand : IRequest<BusinessDto>
    {
        public CreateBusinessDto CreateBusinessDto { get; set; } = new();
    }
}
