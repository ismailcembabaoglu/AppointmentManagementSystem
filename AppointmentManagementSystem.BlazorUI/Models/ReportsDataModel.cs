namespace AppointmentManagementSystem.BlazorUI.Models
{
    public class ReportsDataModel
    {
        public List<RevenueByMonthModel>? RevenueByMonth { get; set; } = new();
        public List<AppointmentsByStatusModel>? AppointmentsByStatus { get; set; } = new();
        public List<TopBusinessesModel>? TopBusinesses { get; set; } = new();
        public List<TopServicesModel>? TopServices { get; set; } = new();
        public List<CategoryDistributionModel>? CategoryDistribution { get; set; } = new();
    }

    public class RevenueByMonthModel
    {
        public string? Month { get; set; } = string.Empty;
        public decimal? Revenue { get; set; } = 0;
        public int? PaymentCount { get; set; } = 0;
    }

    public class AppointmentsByStatusModel
    {
        public string? Status { get; set; } = string.Empty;
        public int? Count { get; set; } = 0;
        public decimal? Percentage { get; set; } = 0;
    }

    public class TopBusinessesModel
    {
        public int? BusinessId { get; set; } = 0;
        public string? BusinessName { get; set; } = string.Empty;
        public int? TotalAppointments { get; set; } = 0;
        public decimal? TotalRevenue { get; set; } = 0;
        public decimal? AverageRating { get; set; } = 0;
    }

    public class TopServicesModel
    {
        public int? ServiceId { get; set; } = 0;
        public string? ServiceName { get; set; } = string.Empty;
        public string? BusinessName { get; set; } = string.Empty;
        public int? BookingCount { get; set; } = 0;
        public decimal? TotalRevenue { get; set; } = 0;
    }

    public class CategoryDistributionModel
    {
        public string? CategoryName { get; set; } = string.Empty;
        public int? BusinessCount { get; set; }
        public int? AppointmentCount { get; set; }
    }
}