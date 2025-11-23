namespace AppointmentManagementSystem.Application.DTOs.Ai
{
    public class AiBusinessRecommendationDto
    {
        public int BusinessId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string? City { get; set; }
        public string? District { get; set; }
        public double AverageRating { get; set; }
        public int TotalRatings { get; set; }
        public List<string> HighlightedReviews { get; set; } = new();
    }
}
