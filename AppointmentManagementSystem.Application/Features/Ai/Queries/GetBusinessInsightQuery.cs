using AppointmentManagementSystem.Application.DTOs.Ai;
using MediatR;

namespace AppointmentManagementSystem.Application.Features.Ai.Queries
{
    public class GetBusinessInsightQuery : IRequest<AiBusinessInsightResponseDto>
    {
        public int BusinessId { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
