using AppointmentManagementSystem.Application.DTOs;
using AppointmentManagementSystem.Application.Models;
using MediatR;

namespace AppointmentManagementSystem.Application.Features.Categories.Queries
{
    public class GetAllCategoriesQuery : PaginationParameters, IRequest<List<CategoryDto>>
    {
    }
}
