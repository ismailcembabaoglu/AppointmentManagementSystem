using AppointmentManagementSystem.Application.DTOs;

namespace AppointmentManagementSystem.Maui.Models
{
    public class AvailableEmployeesResponse
    {
        public List<EmployeeDto> Employees { get; set; } = new();
        public DateTime? NextAvailableDateTime { get; set; }
        public string? NextAvailableMessage { get; set; }
        public bool HasAvailableEmployees => Employees.Any();
    }
}
