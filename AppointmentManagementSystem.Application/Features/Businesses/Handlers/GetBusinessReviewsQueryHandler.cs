using AppointmentManagementSystem.Application.DTOs;
using AppointmentManagementSystem.Application.Features.Businesses.Queries;
using AppointmentManagementSystem.Domain.Entities;
using AppointmentManagementSystem.Domain.Interfaces;
using MediatR;
using System.Linq;

namespace AppointmentManagementSystem.Application.Features.Businesses.Handlers
{
    public class GetBusinessReviewsQueryHandler : IRequestHandler<GetBusinessReviewsQuery, List<BusinessReviewDto>>
    {
        private readonly IAppointmentRepository _appointmentRepository;

        public GetBusinessReviewsQueryHandler(IAppointmentRepository appointmentRepository)
        {
            _appointmentRepository = appointmentRepository;
        }

        public async Task<List<BusinessReviewDto>> Handle(GetBusinessReviewsQuery request, CancellationToken cancellationToken)
        {
            var appointments = await _appointmentRepository.GetAllWithDetailsAsync(businessId: request.BusinessId);
            
            var reviews = appointments
                .Where(a => a.Rating.HasValue 
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
                    ServiceName = a.AppointmentServices.Any()
                        ? string.Join(", ", a.AppointmentServices.Select(s => s.ServiceName))
                        : a.Service != null ? a.Service.Name : ""
                })
                .ToList();

            return reviews;
        }
    }
}
