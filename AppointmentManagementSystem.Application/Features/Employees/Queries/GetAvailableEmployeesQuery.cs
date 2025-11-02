using AppointmentManagementSystem.Application.DTOs;
using MediatR;

namespace AppointmentManagementSystem.Application.Features.Employees.Queries
{
    public class GetAvailableEmployeesQuery : IRequest<List<EmployeeDto>>
    {
        public int BusinessId { get; set; }
        public DateTime SelectedDate { get; set; }
        public TimeSpan SelectedTime { get; set; }
        public int TotalDurationMinutes { get; set; }
    }
}
