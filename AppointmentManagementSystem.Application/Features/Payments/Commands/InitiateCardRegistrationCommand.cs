using AppointmentManagementSystem.Application.DTOs;
using AppointmentManagementSystem.Application.Shared;
using MediatR;

namespace AppointmentManagementSystem.Application.Features.Payments.Commands
{
    public class InitiateCardRegistrationCommand : IRequest<Result<CardRegistrationResponseDto>>
    {
        public string Email { get; set; } = string.Empty;
        public string BusinessName { get; set; } = string.Empty;
        public int BusinessId { get; set; }
        public string UserIp { get; set; } = string.Empty;
        public decimal Amount { get; set; } = 700m;
        public string Description { get; set; } = "Aylık Abonelik Ücreti";
        public bool IsCardUpdate { get; set; }
    }
}