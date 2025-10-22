using AppointmentManagementSystem.Application.DTOs;
using MediatR;

namespace AppointmentManagementSystem.Application.Features.Businesses.Queries
{
    public class GetAllBusinessesQuery : IRequest<List<BusinessDto>>
    {
        public int? CategoryId { get; set; }
        public string? SearchTerm { get; set; }
    }
}
