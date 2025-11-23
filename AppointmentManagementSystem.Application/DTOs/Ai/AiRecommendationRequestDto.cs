namespace AppointmentManagementSystem.Application.DTOs.Ai
{
    public class AiRecommendationRequestDto
    {
        public string Message { get; set; } = string.Empty;
        public string? City { get; set; }
        public string? District { get; set; }
        public int? CategoryId { get; set; }
        public int MaxResults { get; set; } = 3;
    }
}
