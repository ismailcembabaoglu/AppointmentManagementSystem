using AppointmentManagementSystem.Application.DTOs;
using AppointmentManagementSystem.Application.Features.Businesses.Queries;
using AppointmentManagementSystem.Domain.Entities;
using AppointmentManagementSystem.Domain.Interfaces;
using MediatR;

namespace AppointmentManagementSystem.Application.Features.Businesses.Handlers
{
    public class GetBusinessReviewsQueryHandler : IRequestHandler<GetBusinessReviewsQuery, List<BusinessReviewDto>>
    {
        private readonly IRepository<Appointment> _appointmentRepository;

        public GetBusinessReviewsQueryHandler(IRepository<Appointment> appointmentRepository)
        {
            _appointmentRepository = appointmentRepository;
        }

        public async Task<List<BusinessReviewDto>> Handle(GetBusinessReviewsQuery request, CancellationToken cancellationToken)
        {
            var appointments = await _appointmentRepository.GetAllAsync();
            
            var reviews = appointments
                .Where(a => a.BusinessId == request.BusinessId 
                    && a.Rating.HasValue 
                    && !a.IsDeleted
                    && a.Status == "Completed")
                .OrderByDescending(a => a.RatingDate)
                .Select(a => new BusinessReviewDto
                {
                    AppointmentId = a.Id,
                    CustomerName = a.Customer != null ? a.Customer.Name : "Anonim",
                    Rating = a.Rating!.Value,
                    Review = a.Review,
                    RatingDate = a.RatingDate ?? a.UpdatedAt,
                    ServiceName = a.Service != null ? a.Service.Name : ""
                })
                .ToList();

            return reviews;
        }
    }
}
