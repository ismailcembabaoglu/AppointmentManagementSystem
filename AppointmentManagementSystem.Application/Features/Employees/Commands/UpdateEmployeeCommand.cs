using AppointmentManagementSystem.Application.DTOs;
using MediatR;

namespace AppointmentManagementSystem.Application.Features.Employees.Commands
{
    public class UpdateEmployeeCommand : IRequest<EmployeeDto?>
    {
        public int Id { get; set; }
        public CreateEmployeeDto CreateEmployeeDto { get; set; } = new();
    }
}
