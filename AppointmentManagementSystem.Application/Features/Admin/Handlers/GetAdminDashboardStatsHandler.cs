using AppointmentManagementSystem.Application.Features.Admin.Queries;
using AppointmentManagementSystem.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AppointmentManagementSystem.Application.Features.Admin.Handlers
{
    public class GetAdminDashboardStatsHandler : IRequestHandler<GetAdminDashboardStatsQuery, AdminDashboardStatsDto>
    {
        private readonly IBusinessRepository _businessRepository;
        private readonly IRepository<AppointmentManagementSystem.Domain.Entities.User> _userRepository;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IBusinessSubscriptionRepository _subscriptionRepository;

        public GetAdminDashboardStatsHandler(
            IBusinessRepository businessRepository,
            IRepository<AppointmentManagementSystem.Domain.Entities.User> userRepository,
            IAppointmentRepository appointmentRepository,
            IPaymentRepository paymentRepository,
            IBusinessSubscriptionRepository subscriptionRepository)
        {
            _businessRepository = businessRepository;
            _userRepository = userRepository;
            _appointmentRepository = appointmentRepository;
            _paymentRepository = paymentRepository;
            _subscriptionRepository = subscriptionRepository;
        }

        public async Task<AdminDashboardStatsDto> Handle(GetAdminDashboardStatsQuery request, CancellationToken cancellationToken)
        {
            var businesses = await _businessRepository.GetAllAsync();
            var users = await _userRepository.GetAllAsync();
            var appointments = await _appointmentRepository.GetAllAsync();
            var payments = await _paymentRepository.GetAllAsync();
            var subscriptions = await _subscriptionRepository.GetAllAsync();

            var today = DateTime.Today;
            var thisMonth = new DateTime(today.Year, today.Month, 1);

            return new AdminDashboardStatsDto
            {
                TotalBusinesses = businesses.Count(),
                ActiveBusinesses = businesses.Count(b => b.IsActive),
                InactiveBusinesses = businesses.Count(b => !b.IsActive),
                TotalCustomers = users.Count(u => u.Role == "Customer"),
                TotalAppointments = appointments.Count(),
                TodayAppointments = appointments.Count(a => a.AppointmentDate.Date == today),
                PendingAppointments = appointments.Count(a => a.Status == "Pending"),
                TotalRevenue = payments.Where(p => p.Status == "Success").Sum(p => p.Amount),
                MonthlyRevenue = payments.Where(p => p.Status == "Success" && p.PaymentDate >= thisMonth).Sum(p => p.Amount),
                ActiveSubscriptions = subscriptions.Count(s => s.Status == "Active"),
                ExpiredSubscriptions = subscriptions.Count(s => s.Status == "Expired" || s.Status == "Cancelled")
            };
        }
    }
}