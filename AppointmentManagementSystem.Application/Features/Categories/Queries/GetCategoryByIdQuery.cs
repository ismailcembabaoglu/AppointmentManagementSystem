using AppointmentManagementSystem.Application.DTOs;
using MediatR;

namespace AppointmentManagementSystem.Application.Features.Categories.Queries
{
    public class GetCategoryByIdQuery : IRequest<CategoryDto?>
    {
        public int Id { get; set; }
    }
}
