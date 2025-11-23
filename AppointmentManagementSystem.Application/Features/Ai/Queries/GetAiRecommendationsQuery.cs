using AppointmentManagementSystem.Application.DTOs.Ai;
using MediatR;

namespace AppointmentManagementSystem.Application.Features.Ai.Queries
{
    public class GetAiRecommendationsQuery : IRequest<AiRecommendationResponseDto>
    {
        public string Message { get; set; } = string.Empty;
        public string? City { get; set; }
        public string? District { get; set; }
        public int? CategoryId { get; set; }
        public int MaxResults { get; set; } = 3;
    }
}
