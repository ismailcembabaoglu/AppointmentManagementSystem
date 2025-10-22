using AppointmentManagementSystem.Application.DTOs;
using MediatR;

namespace AppointmentManagementSystem.Application.Features.Employees.Commands
{
    public class CreateEmployeeCommand : IRequest<EmployeeDto>
    {
        public CreateEmployeeDto CreateEmployeeDto { get; set; } = new();
    }
}
