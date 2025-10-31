using AppointmentManagementSystem.Application.DTOs;
using AppointmentManagementSystem.Application.Features.Businesses.Queries;
using AppointmentManagementSystem.Domain.Interfaces;
using AppointmentManagementSystem.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AppointmentManagementSystem.Application.Features.Businesses.Handlers
{
    public class GetBusinessReviewsQueryHandler : IRequestHandler<GetBusinessReviewsQuery, List<BusinessReviewDto>>
    {
        private readonly AppointmentDbContext _context;

        public GetBusinessReviewsQueryHandler(AppointmentDbContext context)
        {
            _context = context;
        }

        public async Task<List<BusinessReviewDto>> Handle(GetBusinessReviewsQuery request, CancellationToken cancellationToken)
        {
            var reviews = await _context.Set<Domain.Entities.Appointment>()
                .Include(a => a.Customer)
                .Include(a => a.Service)
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
                .ToListAsync(cancellationToken);

            return reviews;
        }
    }
}
