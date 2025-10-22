using AppointmentManagementSystem.Application.DTOs;
using MediatR;

namespace AppointmentManagementSystem.Application.Features.Employees.Queries
{
    public class GetAllEmployeesQuery : IRequest<List<EmployeeDto>>
    {
        public int? BusinessId { get; set; }
    }
}
