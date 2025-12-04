using AppointmentManagementSystem.Application.Features.Admin.Queries;
using AppointmentManagementSystem.Domain.Interfaces;
using MediatR;

namespace AppointmentManagementSystem.Application.Features.Admin.Handlers
{
    public class GetReportsDataHandler : IRequestHandler<GetReportsDataQuery, ReportsDataDto>
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IBusinessRepository _businessRepository;
        private readonly IServiceRepository _serviceRepository;
        private readonly ICategoryRepository _categoryRepository;

        public GetReportsDataHandler(
            IPaymentRepository paymentRepository,
            IAppointmentRepository appointmentRepository,
            IBusinessRepository businessRepository,
            IServiceRepository serviceRepository,
            ICategoryRepository categoryRepository)
        {
            _paymentRepository = paymentRepository;
            _appointmentRepository = appointmentRepository;
            _businessRepository = businessRepository;
            _serviceRepository = serviceRepository;
            _categoryRepository = categoryRepository;
        }

        public async Task<ReportsDataDto> Handle(GetReportsDataQuery request, CancellationToken cancellationToken)
        {
            var startDate = request.StartDate ?? DateTime.Now.AddMonths(-12);
            var endDate = request.EndDate ?? DateTime.Now;

            var payments = await _paymentRepository.GetAllAsync();
            var appointments = await _appointmentRepository.GetAllAsync();
            var businesses = await _businessRepository.GetAllAsync();
            var services = await _serviceRepository.GetAllAsync();
            var categories = await _categoryRepository.GetAllAsync();

            var filteredPayments = payments.Where(p => p.PaymentDate >= startDate && p.PaymentDate <= endDate && p.Status == "Success");
            var filteredAppointments = appointments.Where(a => a.CreatedAt >= startDate && a.CreatedAt <= endDate);

            // Revenue by Month
            var revenueByMonth = filteredPayments
                .GroupBy(p => new { p.PaymentDate.Year, p.PaymentDate.Month })
                .Select(g => new RevenueByMonthDto
                {
                    Month = $"{g.Key.Year}-{g.Key.Month:D2}",
                    Revenue = g.Sum(p => p.Amount),
                    PaymentCount = g.Count()
                })
                .OrderBy(r => r.Month)
                .ToList();

            // Appointments by Status
            var totalAppointments = filteredAppointments.Count();
            var appointmentsByStatus = filteredAppointments
                .GroupBy(a => a.Status)
                .Select(g => new AppointmentsByStatusDto
                {
                    Status = g.Key,
                    Count = g.Count(),
                    Percentage = totalAppointments > 0 ? (decimal)g.Count() / totalAppointments * 100 : 0
                })
                .ToList();

            // Top Businesses
            var topBusinesses = new List<TopBusinessesDto>();
            foreach (var business in businesses.Take(10))
            {
                var businessAppointments = appointments.Where(a => a.BusinessId == business.Id);
                var businessPayments = payments.Where(p => p.BusinessId == business.Id && p.Status == "Success");
                var avgRating = await _businessRepository.GetAverageRatingAsync(business.Id);

                topBusinesses.Add(new TopBusinessesDto
                {
                    BusinessId = business.Id,
                    BusinessName = business.Name,
                    TotalAppointments = businessAppointments.Count(),
                    TotalRevenue = businessPayments.Sum(p => p.Amount),
                    AverageRating = avgRating.HasValue ? (decimal)avgRating.Value : null
                });
            }
            topBusinesses = topBusinesses.OrderByDescending(b => b.TotalRevenue).Take(10).ToList();

            // Top Services
            var appointmentsWithDetails = await _appointmentRepository.GetAllWithDetailsAsync();
            var topServices = appointmentsWithDetails
                .Where(a => a.AppointmentDate >= startDate && a.AppointmentDate <= endDate)
                .SelectMany(a => a.AppointmentServices.Select(s => new { Appointment = a, Service = s }))
                .GroupBy(x => new { x.Service.ServiceId, x.Service.ServiceName, BusinessName = x.Appointment.Business?.Name })
                .Select(g => new TopServicesDto
                {
                    ServiceId = g.Key.ServiceId,
                    ServiceName = g.Key.ServiceName ?? "N/A",
                    BusinessName = g.Key.BusinessName ?? "N/A",
                    BookingCount = g.Count(),
                    TotalRevenue = g.Sum(x => x.Service.Price)
                })
                .OrderByDescending(s => s.BookingCount)
                .Take(10)
                .ToList();

            // Category Distribution
            var categoryDistribution = new List<CategoryDistributionDto>();
            foreach (var category in categories)
            {
                var categoryBusinesses = businesses.Where(b => b.CategoryId == category.Id).ToList();
                var categoryAppointments = appointments.Where(a => categoryBusinesses.Any(b => b.Id == a.BusinessId));

                categoryDistribution.Add(new CategoryDistributionDto
                {
                    CategoryName = category.Name,
                    BusinessCount = categoryBusinesses.Count,
                    AppointmentCount = categoryAppointments.Count()
                });
            }

            return new ReportsDataDto
            {
                RevenueByMonth = revenueByMonth,
                AppointmentsByStatus = appointmentsByStatus,
                TopBusinesses = topBusinesses,
                TopServices = topServices,
                CategoryDistribution = categoryDistribution
            };
        }
    }
}