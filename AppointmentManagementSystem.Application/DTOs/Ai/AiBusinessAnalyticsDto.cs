using System;
using System.Collections.Generic;

namespace AppointmentManagementSystem.Application.DTOs.Ai
{
    public class AiBusinessAnalyticsDto
    {
        public string BusinessName { get; set; } = string.Empty;
        public string? CategoryName { get; set; }
        public string? City { get; set; }
        public string? District { get; set; }
        public AiBusinessStatsDto Stats { get; set; } = new();
        public List<AiBusinessServiceDto> Services { get; set; } = new();
        public List<AiBusinessAppointmentDto> RecentAppointments { get; set; } = new();
        public List<AiBusinessReviewDto> RecentReviews { get; set; } = new();
    }

    public class AiBusinessStatsDto
    {
        public int TotalAppointments { get; set; }
        public int CompletedAppointments { get; set; }
        public int PendingAppointments { get; set; }
        public int CancelledAppointments { get; set; }
        public double? AverageRating { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    public class AiBusinessServiceDto
    {
        public int ServiceId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal? Price { get; set; }
        public int BookingCount { get; set; }
        public int CompletedCount { get; set; }
        public decimal Revenue { get; set; }
        public double? AverageRating { get; set; }
    }

    public class AiBusinessAppointmentDto
    {
        public DateTime AppointmentDate { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? CustomerName { get; set; }
        public string? EmployeeName { get; set; }
        public decimal Price { get; set; }
        public int? Rating { get; set; }
    }

    public class AiBusinessReviewDto
    {
        public string? CustomerName { get; set; }
        public string? ServiceName { get; set; }
        public int Rating { get; set; }
        public string? Review { get; set; }
        public DateTime? RatingDate { get; set; }
    }
}
