using AppointmentManagementSystem.Application.Features.Admin.Queries;
using AppointmentManagementSystem.Domain.Interfaces;
using MediatR;

namespace AppointmentManagementSystem.Application.Features.Admin.Handlers
{
    public class GetBusinessDetailAdminHandler : IRequestHandler<GetBusinessDetailAdminQuery, BusinessDetailAdminDto>
    {
        private readonly IBusinessRepository _businessRepository;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IServiceRepository _serviceRepository;
        private readonly IBusinessSubscriptionRepository _subscriptionRepository;

        public GetBusinessDetailAdminHandler(
            IBusinessRepository businessRepository,
            IAppointmentRepository appointmentRepository,
            IEmployeeRepository employeeRepository,
            IServiceRepository serviceRepository,
            IBusinessSubscriptionRepository subscriptionRepository)
        {
            _businessRepository = businessRepository;
            _appointmentRepository = appointmentRepository;
            _employeeRepository = employeeRepository;
            _serviceRepository = serviceRepository;
            _subscriptionRepository = subscriptionRepository;
        }

        public async Task<BusinessDetailAdminDto> Handle(GetBusinessDetailAdminQuery request, CancellationToken cancellationToken)
        {
            var business = await _businessRepository.GetByIdWithDetailsAsync(request.BusinessId);
            if (business == null)
                throw new Exception("Business not found");

            var appointments = await _appointmentRepository.GetByBusinessAsync(business.Id);
            var employees = await _employeeRepository.GetByBusinessAsync(business.Id);
            var services = await _serviceRepository.GetByBusinessIdAsync(business.Id);
            var subscription = await _subscriptionRepository.GetByBusinessIdAsync(business.Id);
            var avgRating = await _businessRepository.GetAverageRatingAsync(business.Id);

            return new BusinessDetailAdminDto
            {
                Id = business.Id,
                Name = business.Name,
                Description = business.Description,
                Email = business.Email,
                Phone = business.Phone,
                Address = business.Address,
                City = business.City,
                District = business.District,
                IsActive = business.IsActive,
                CreatedAt = business.CreatedAt,
                CategoryName = business.Category?.Name,
                TotalAppointments = appointments.Count(),
                CompletedAppointments = appointments.Count(a => a.Status == "Completed"),
                CancelledAppointments = appointments.Count(a => a.Status == "Cancelled"),
                TotalEmployees = employees.Count(),
                TotalServices = services.Count(),
                AverageRating = avgRating.HasValue ? (decimal)avgRating.Value : null,
                TotalReviews = appointments.Count(a => a.Rating.HasValue),
                SubscriptionStatus = subscription?.Status ?? "None",
                SubscriptionStartDate = subscription?.StartDate,
                NextBillingDate = subscription?.NextBillingDate,
                CardLastFourDigits = subscription?.CardLastFourDigits,
                CardType = subscription?.CardType,
                AutoRenewal = subscription?.AutoRenewal ?? false
            };
        }
    }
}