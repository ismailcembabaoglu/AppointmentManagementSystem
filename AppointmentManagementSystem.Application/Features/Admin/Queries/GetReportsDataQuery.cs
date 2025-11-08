using MediatR;

namespace AppointmentManagementSystem.Application.Features.Admin.Queries
{
    public class GetReportsDataQuery : IRequest<ReportsDataDto>
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class ReportsDataDto
    {
        public List<RevenueByMonthDto> RevenueByMonth { get; set; } = new();
        public List<AppointmentsByStatusDto> AppointmentsByStatus { get; set; } = new();
        public List<TopBusinessesDto> TopBusinesses { get; set; } = new();
        public List<TopServicesDto> TopServices { get; set; } = new();
        public List<CategoryDistributionDto> CategoryDistribution { get; set; } = new();
    }

    public class RevenueByMonthDto
    {
        public string Month { get; set; } = string.Empty;
        public decimal Revenue { get; set; }
        public int PaymentCount { get; set; }
    }

    public class AppointmentsByStatusDto
    {
        public string Status { get; set; } = string.Empty;
        public int Count { get; set; }
        public decimal Percentage { get; set; }
    }

    public class TopBusinessesDto
    {
        public int BusinessId { get; set; }
        public string BusinessName { get; set; } = string.Empty;
        public int TotalAppointments { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal? AverageRating { get; set; }
    }

    public class TopServicesDto
    {
        public int ServiceId { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public string BusinessName { get; set; } = string.Empty;
        public int BookingCount { get; set; }
        public decimal? TotalRevenue { get; set; }
    }

    public class CategoryDistributionDto
    {
        public string CategoryName { get; set; } = string.Empty;
        public int BusinessCount { get; set; }
        public int AppointmentCount { get; set; }
    }
}