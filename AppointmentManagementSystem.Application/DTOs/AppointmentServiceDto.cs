namespace AppointmentManagementSystem.Application.DTOs
{
    public class AppointmentServiceDto
    {
        public int ServiceId { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int DurationMinutes { get; set; }
    }
}
