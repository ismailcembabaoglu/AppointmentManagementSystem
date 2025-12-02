namespace AppointmentManagementSystem.Application.DTOs
{
    public class BusinessDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Website { get; set; }
        public string? City { get; set; }
        public string? District { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string? MapEmbedCode { get; set; }
        public bool IsActive { get; set; }
        public double? AverageRating { get; set; }
        public int TotalRatings { get; set; }
        public List<string> PhotoUrls { get; set; } = new();
    }
}
