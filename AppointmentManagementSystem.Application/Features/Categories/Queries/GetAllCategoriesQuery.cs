using AppointmentManagementSystem.Application.DTOs;
using AppointmentManagementSystem.Application.Models;
using AppointmentManagementSystem.Application.Shared;
using MediatR;

namespace AppointmentManagementSystem.Application.Features.Categories.Queries
{
    public class GetAllCategoriesQuery : PaginationParameters, IRequest<PaginatedResult<CategoryDto>>
    {
    }
}
