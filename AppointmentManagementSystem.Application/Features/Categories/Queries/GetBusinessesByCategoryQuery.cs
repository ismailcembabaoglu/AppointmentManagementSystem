using AppointmentManagementSystem.Application.DTOs;
using AppointmentManagementSystem.Application.Models;
using MediatR;

namespace AppointmentManagementSystem.Application.Features.Categories.Queries
{
    public class GetBusinessesByCategoryQuery : PaginationParameters, IRequest<List<BusinessDto>>
    {
        public int CategoryId { get; set; }
    }
}
