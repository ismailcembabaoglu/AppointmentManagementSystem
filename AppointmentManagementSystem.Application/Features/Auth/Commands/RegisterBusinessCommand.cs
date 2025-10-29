using AppointmentManagementSystem.Application.DTOs;
using MediatR;

namespace AppointmentManagementSystem.Application.Features.Auth.Commands
{
    public class RegisterBusinessCommand : IRequest<AuthResponseDto>
    {
        public RegisterBusinessDto RegisterBusinessDto { get; set; } = null!;
    }
}
