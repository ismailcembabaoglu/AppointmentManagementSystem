using AppointmentManagementSystem.Application.DTOs;
using AppointmentManagementSystem.Application.Shared;
using MediatR;

namespace AppointmentManagementSystem.Application.Features.Auth.Commands
{
    public class LoginCommand : IRequest<BaseResponse<AuthResponseDto>>
    {
        public LoginDto LoginDto { get; set; } = new();
    }
}
