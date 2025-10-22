using AppointmentManagementSystem.Application.DTOs;
using MediatR;

namespace AppointmentManagementSystem.Application.Features.Employees.Queries
{
    public class GetEmployeeByIdQuery : IRequest<EmployeeDto?>
    {
        public int Id { get; set; }
    }
}
