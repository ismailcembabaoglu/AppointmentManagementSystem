using AppointmentManagementSystem.Application.DTOs;
using AppointmentManagementSystem.Application.Models;
using AppointmentManagementSystem.Application.Shared;
using MediatR;

namespace AppointmentManagementSystem.Application.Features.Categories.Queries
{
    public class GetBusinessesByCategoryQuery : PaginationParameters, IRequest<PaginatedResult<BusinessDto>>
    {
        public int CategoryId { get; set; }
    }
}
