using MediatR;

namespace AppointmentManagementSystem.Application.Features.Admin.Queries
{
    public class GetAdminDashboardStatsQuery : IRequest<AdminDashboardStatsDto>
    {
    }

    public class AdminDashboardStatsDto
    {
        public int TotalBusinesses { get; set; }
        public int ActiveBusinesses { get; set; }
        public int InactiveBusinesses { get; set; }
        public int TotalCustomers { get; set; }
        public int TotalAppointments { get; set; }
        public int TodayAppointments { get; set; }
        public int PendingAppointments { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal MonthlyRevenue { get; set; }
        public int ActiveSubscriptions { get; set; }
        public int ExpiredSubscriptions { get; set; }
    }
}