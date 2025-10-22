namespace AppointmentManagementSystem.Application.DTOs
{
    public class ServiceDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public int DurationMinutes { get; set; }
        public int BusinessId { get; set; }
        public string BusinessName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public List<string> PhotoUrls { get; set; } = new();
    }
}
