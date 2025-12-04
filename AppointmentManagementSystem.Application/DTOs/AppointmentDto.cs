using System;
using System.Collections.Generic;

namespace AppointmentManagementSystem.Application.DTOs
{
    public class AppointmentDto
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public int BusinessId { get; set; }
        public string BusinessName { get; set; } = string.Empty;
        public int? EmployeeId { get; set; }
        public string? EmployeeName { get; set; }
        public int ServiceId { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public List<int> ServiceIds { get; set; } = new();
        public List<AppointmentServiceDto> Services { get; set; } = new();
        public string ServicesSummary { get; set; } = string.Empty;
        public decimal TotalPrice { get; set; }
        public int TotalDurationMinutes { get; set; }
        public DateTime AppointmentDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string? Notes { get; set; }
        public string Status { get; set; } = string.Empty;
        public int? Rating { get; set; }
        public string? Review { get; set; }
        public DateTime? RatingDate { get; set; }
        public List<string> PhotoUrls { get; set; } = new();
    }
}
