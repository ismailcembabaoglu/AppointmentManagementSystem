using AppointmentManagementSystem.Application.DTOs;

namespace AppointmentManagementSystem.Application.DTOs
{
    public class AvailableEmployeesResponseDto
    {
        public List<EmployeeDto> Employees { get; set; } = new();
        public DateTime? NextAvailableDateTime { get; set; }
        public string? NextAvailableMessage { get; set; }
        public bool HasAvailableEmployees => Employees.Any();
    }
}
