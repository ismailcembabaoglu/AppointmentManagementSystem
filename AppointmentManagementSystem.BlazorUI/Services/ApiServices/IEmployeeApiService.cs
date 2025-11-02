using AppointmentManagementSystem.Application.DTOs;
using AppointmentManagementSystem.BlazorUI.Models;

namespace AppointmentManagementSystem.BlazorUI.Services.ApiServices
{
    public interface IEmployeeApiService
    {
        Task<ApiResponse<List<EmployeeDto>>> GetAllEmployeesAsync(int? businessId = null);
        Task<ApiResponse<List<EmployeeDto>>> GetAvailableEmployeesAsync(int businessId, DateTime selectedDate, TimeSpan selectedTime, int totalDurationMinutes);
        Task<ApiResponse<EmployeeDto?>> GetEmployeeByIdAsync(int id);
        Task<ApiResponse<EmployeeDto>> CreateEmployeeAsync(CreateEmployeeDto createEmployeeDto);
        Task<ApiResponse<EmployeeDto?>> UpdateEmployeeAsync(int id, CreateEmployeeDto updateEmployeeDto);
        Task<ApiResponse<bool>> DeleteEmployeeAsync(int id);
    }
}
