using AppointmentManagementSystem.Application.DTOs;
using AppointmentManagementSystem.Application.Models;
using MediatR;

namespace AppointmentManagementSystem.Application.Features.Businesses.Queries
{
    public class GetAllBusinessesQuery : PaginationParameters, IRequest<List<BusinessDto>>
    {
        public int? CategoryId { get; set; }
        public string? SearchTerm { get; set; }
        public string? City { get; set; }
        public string? District { get; set; }
        public double? MinRating { get; set; }
    }
}
