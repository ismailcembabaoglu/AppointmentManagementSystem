using AppointmentManagementSystem.Application.Features.Admin.Queries;
using AppointmentManagementSystem.Domain.Interfaces;
using MediatR;

namespace AppointmentManagementSystem.Application.Features.Admin.Handlers
{
    public class GetAllBusinessesAdminHandler : IRequestHandler<GetAllBusinessesAdminQuery, List<BusinessAdminDto>>
    {
        private readonly IBusinessRepository _businessRepository;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IBusinessSubscriptionRepository _subscriptionRepository;

        public GetAllBusinessesAdminHandler(
            IBusinessRepository businessRepository,
            IAppointmentRepository appointmentRepository,
            IEmployeeRepository employeeRepository,
            IBusinessSubscriptionRepository subscriptionRepository)
        {
            _businessRepository = businessRepository;
            _appointmentRepository = appointmentRepository;
            _employeeRepository = employeeRepository;
            _subscriptionRepository = subscriptionRepository;
        }

        public async Task<List<BusinessAdminDto>> Handle(GetAllBusinessesAdminQuery request, CancellationToken cancellationToken)
        {
            var businesses = await _businessRepository.GetAllWithDetailsAsync(
                categoryId: request.CategoryId,
                searchTerm: request.SearchTerm
            );

            if (request.IsActive.HasValue)
            {
                businesses = businesses.Where(b => b.IsActive == request.IsActive.Value);
            }

            var result = new List<BusinessAdminDto>();

            foreach (var business in businesses)
            {
                var appointments = await _appointmentRepository.GetByBusinessAsync(business.Id);
                var employees = await _employeeRepository.GetByBusinessAsync(business.Id);
                var subscription = await _subscriptionRepository.GetByBusinessIdAsync(business.Id);
                var avgRating = await _businessRepository.GetAverageRatingAsync(business.Id);
                var totalReviews = appointments.Count(a => a.Rating.HasValue);

                result.Add(new BusinessAdminDto
                {
                    Id = business.Id,
                    Name = business.Name,
                    CategoryName = business.Category?.Name,
                    Email = business.Email,
                    Phone = business.Phone,
                    IsActive = business.IsActive,
                    CreatedAt = business.CreatedAt,
                    TotalAppointments = appointments.Count(),
                    TotalEmployees = employees.Count(),
                    AverageRating = avgRating.HasValue ? (decimal)avgRating.Value : null,
                    TotalReviews = totalReviews,
                    SubscriptionStatus = subscription?.Status ?? "None",
                    NextBillingDate = subscription?.NextBillingDate
                });
            }

            return result.OrderByDescending(b => b.CreatedAt).ToList();
        }
    }
}