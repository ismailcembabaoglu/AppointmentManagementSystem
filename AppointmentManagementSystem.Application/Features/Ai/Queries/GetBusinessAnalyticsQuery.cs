using AppointmentManagementSystem.Application.DTOs.Ai;
using MediatR;

namespace AppointmentManagementSystem.Application.Features.Ai.Queries
{
    public class GetBusinessAnalyticsQuery : IRequest<AiBusinessAnalyticsDto?>
    {
        public int BusinessId { get; set; }
    }
}
