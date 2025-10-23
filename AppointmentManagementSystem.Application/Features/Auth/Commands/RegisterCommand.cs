using AppointmentManagementSystem.Application.DTOs;
using AppointmentManagementSystem.Application.Shared;
using MediatR;

namespace AppointmentManagementSystem.Application.Features.Auth.Commands
{
    public class RegisterCommand : IRequest<AuthResponseDto>
    {
        public RegisterDto RegisterDto { get; set; } = new();
        public CreateBusinessDto? BusinessDto { get; set; } // Business rolü için
    }
}
