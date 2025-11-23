namespace AppointmentManagementSystem.Application.DTOs.Ai
{
    public class AiRecommendationResponseDto
    {
        public string Reply { get; set; } = string.Empty;
        public List<AiBusinessRecommendationDto> Recommendations { get; set; } = new();
    }
}
