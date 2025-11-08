namespace AppointmentManagementSystem.BlazorUI.Models
{
    public class AppointmentAdminModel
    {
        public int Id { get; set; }
        public DateTime AppointmentDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string Status { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string? CustomerEmail { get; set; }
        public string? CustomerPhone { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public decimal? ServicePrice { get; set; }
        public string? EmployeeName { get; set; }
        public string? Notes { get; set; }
        public int? Rating { get; set; }
        public string? Review { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}