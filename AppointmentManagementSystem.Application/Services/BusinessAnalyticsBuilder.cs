using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AppointmentManagementSystem.Application.DTOs.Ai;
using AppointmentManagementSystem.Domain.Interfaces;

namespace AppointmentManagementSystem.Application.Services
{
    public class BusinessAnalyticsBuilder : IBusinessAnalyticsBuilder
    {
        private readonly IBusinessRepository _businessRepository;
        private readonly IServiceRepository _serviceRepository;
        private readonly IAppointmentRepository _appointmentRepository;

        public BusinessAnalyticsBuilder(
            IBusinessRepository businessRepository,
            IServiceRepository serviceRepository,
            IAppointmentRepository appointmentRepository)
        {
            _businessRepository = businessRepository;
            _serviceRepository = serviceRepository;
            _appointmentRepository = appointmentRepository;
        }

        public async Task<AiBusinessAnalyticsDto?> BuildAsync(int businessId, CancellationToken cancellationToken = default)
        {
            var business = await _businessRepository.GetByIdWithDetailsAsync(businessId);
            if (business == null)
            {
                return null;
            }

            var services = (await _serviceRepository.GetByBusinessAsync(businessId)).ToList();
            var appointments = (await _appointmentRepository.GetAllWithDetailsAsync(businessId: businessId)).ToList();

            var reviews = appointments
                .Where(a => a.Rating.HasValue && !string.IsNullOrWhiteSpace(a.Review))
                .OrderByDescending(a => a.RatingDate ?? a.UpdatedAt)
                .Take(15)
                .Select(a => new AiBusinessReviewDto
                {
                    CustomerName = a.Customer != null ? a.Customer.Name : "Anonim",
                    ServiceName = a.Service?.Name,
                    Rating = a.Rating!.Value,
                    Review = a.Review,
                    RatingDate = a.RatingDate ?? a.UpdatedAt
                })
                .ToList();

            var stats = new AiBusinessStatsDto
            {
                TotalAppointments = appointments.Count,
                CompletedAppointments = appointments.Count(a => a.Status == "Completed"),
                PendingAppointments = appointments.Count(a => a.Status == "Pending" || a.Status == "Confirmed"),
                CancelledAppointments = appointments.Count(a => a.Status == "Cancelled"),
                AverageRating = appointments.Where(a => a.Rating.HasValue).Any()
                    ? appointments.Where(a => a.Rating.HasValue).Average(a => a.Rating) : null,
                TotalRevenue = appointments
                    .Where(a => a.Status == "Completed")
                    .Sum(a => a.Service?.Price ?? 0)
            };

            var serviceSummaries = services
                .Select(s =>
                {
                    var ratings = appointments
                        .Where(a => a.ServiceId == s.Id && a.Rating.HasValue)
                        .Select(a => (double)a.Rating!)
                        .ToList();

                    return new AiBusinessServiceDto
                    {
                        ServiceId = s.Id,
                        Name = s.Name,
                        Price = s.Price,
                        BookingCount = appointments.Count(a => a.ServiceId == s.Id),
                        CompletedCount = appointments.Count(a => a.ServiceId == s.Id && a.Status == "Completed"),
                        Revenue = appointments
                            .Where(a => a.ServiceId == s.Id && a.Status == "Completed")
                            .Sum(a => a.Service?.Price ?? 0),
                        AverageRating = ratings.Any() ? ratings.Average() : null
                    };
                })
                .OrderByDescending(s => s.Revenue)
                .ThenByDescending(s => s.BookingCount)
                .ToList();

            var recentAppointments = appointments
                .OrderByDescending(a => a.AppointmentDate)
                .Take(30)
                .Select(a => new AiBusinessAppointmentDto
                {
                    AppointmentDate = a.AppointmentDate.Date + a.StartTime,
                    CustomerName = a.Customer?.Name,
                    EmployeeName = a.Employee?.Name,
                    ServiceName = a.Service?.Name ?? "Bilinmiyor",
                    Status = a.Status,
                    Price = a.Service?.Price ?? 0,
                    Rating = a.Rating
                })
                .ToList();

            return new AiBusinessAnalyticsDto
            {
                BusinessName = business.Name,
                CategoryName = business.Category?.Name,
                City = business.City,
                District = business.District,
                Stats = stats,
                Services = serviceSummaries,
                RecentAppointments = recentAppointments,
                RecentReviews = reviews
            };
        }
    }
}
