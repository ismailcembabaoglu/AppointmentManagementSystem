using AppointmentManagementSystem.Application.DTOs;
using MediatR;

namespace AppointmentManagementSystem.Application.Features.Categories.Queries
{
    public class GetBusinessesByCategoryQuery : IRequest<List<BusinessDto>>
    {
        public int CategoryId { get; set; }
    }
}
