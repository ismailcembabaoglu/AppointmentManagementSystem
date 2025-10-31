using AppointmentManagementSystem.Application.DTOs;
using MediatR;

namespace AppointmentManagementSystem.Application.Features.Businesses.Queries
{
    public class GetBusinessReviewsQuery : IRequest<List<BusinessReviewDto>>
    {
        public int BusinessId { get; set; }
    }
}
