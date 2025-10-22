using AppointmentManagementSystem.Application.DTOs;
using MediatR;

namespace AppointmentManagementSystem.Application.Features.Categories.Commands
{
    public class CreateCategoryCommand : IRequest<CategoryDto>
    {
        public CreateCategoryDto CreateCategoryDto { get; set; } = new();
    }
}
