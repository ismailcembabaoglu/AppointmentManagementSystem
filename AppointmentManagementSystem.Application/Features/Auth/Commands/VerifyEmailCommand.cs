using MediatR;

namespace AppointmentManagementSystem.Application.Features.Auth.Commands
{
    public class VerifyEmailCommand : IRequest<bool>
    {
        public string Token { get; set; } = string.Empty;
    }
}
