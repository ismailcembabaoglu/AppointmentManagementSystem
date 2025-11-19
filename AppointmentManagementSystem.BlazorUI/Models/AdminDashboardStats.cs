namespace AppointmentManagementSystem.BlazorUI.Models
{
    public class AdminDashboardStats
    {
        public int? TotalBusinesses { get; set; } = 0;
        public int? ActiveBusinesses { get; set; } = 0;
        public int? InactiveBusinesses { get; set; } = 0;
        public int? TotalCustomers { get; set; } = 0;
        public int? TotalAppointments { get; set; } = 0;
        public int? TodayAppointments { get; set; } = 0;
        public int? PendingAppointments { get; set; } = 0;
        public decimal? TotalRevenue { get; set; } = 0;
        public decimal? MonthlyRevenue { get; set; } = 0;
        public int? ActiveSubscriptions { get; set; } = 0;
        public int? ExpiredSubscriptions { get; set; } = 0;
    }
}