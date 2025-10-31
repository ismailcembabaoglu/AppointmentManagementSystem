namespace AppointmentManagementSystem.Application.DTOs
{
    public class BusinessReviewDto
    {
        public int AppointmentId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public int Rating { get; set; }
        public string? Review { get; set; }
        public DateTime RatingDate { get; set; }
        public string ServiceName { get; set; } = string.Empty;
    }
}
