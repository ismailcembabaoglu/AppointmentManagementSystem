using AppointmentManagementSystem.Application.DTOs;
using AppointmentManagementSystem.Application.Shared;
using MediatR;

namespace AppointmentManagementSystem.Application.Features.Auth.Commands
{
    public class RegisterCommand : IRequest<BaseResponse<AuthResponseDto>>
    {
        public RegisterDto RegisterDto { get; set; } = new();
    }
}
