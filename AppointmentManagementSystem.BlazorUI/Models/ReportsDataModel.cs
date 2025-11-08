namespace AppointmentManagementSystem.BlazorUI.Models
{
    public class ReportsDataModel
    {
        public List<RevenueByMonthModel> RevenueByMonth { get; set; } = new();
        public List<AppointmentsByStatusModel> AppointmentsByStatus { get; set; } = new();
        public List<TopBusinessesModel> TopBusinesses { get; set; } = new();
        public List<TopServicesModel> TopServices { get; set; } = new();
        public List<CategoryDistributionModel> CategoryDistribution { get; set; } = new();
    }

    public class RevenueByMonthModel
    {
        public string Month { get; set; } = string.Empty;
        public decimal Revenue { get; set; }
        public int PaymentCount { get; set; }
    }

    public class AppointmentsByStatusModel
    {
        public string Status { get; set; } = string.Empty;
        public int Count { get; set; }
        public decimal Percentage { get; set; }
    }

    public class TopBusinessesModel
    {
        public int BusinessId { get; set; }
        public string BusinessName { get; set; } = string.Empty;
        public int TotalAppointments { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal? AverageRating { get; set; }
    }

    public class TopServicesModel
    {
        public int ServiceId { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public string BusinessName { get; set; } = string.Empty;
        public int BookingCount { get; set; }
        public decimal? TotalRevenue { get; set; }
    }

    public class CategoryDistributionModel
    {
        public string CategoryName { get; set; } = string.Empty;
        public int BusinessCount { get; set; }
        public int AppointmentCount { get; set; }
    }
}