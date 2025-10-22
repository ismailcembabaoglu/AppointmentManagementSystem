using AppointmentManagementSystem.Application.DTOs;
using MediatR;

namespace AppointmentManagementSystem.Application.Features.BusinessUsers.Commands
{
    public class CreateBusinessUserCommand : IRequest<BusinessUserDto>
    {
        public CreateBusinessUserDto CreateBusinessUserDto { get; set; } = new();
    }
}
