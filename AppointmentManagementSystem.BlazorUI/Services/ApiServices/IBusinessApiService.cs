using AppointmentManagementSystem.Application.DTOs;
using AppointmentManagementSystem.BlazorUI.Models;

namespace AppointmentManagementSystem.BlazorUI.Services.ApiServices
{
    public interface IBusinessApiService
    {
        Task<ApiResponse<List<BusinessDto>>> GetAllBusinessesAsync(int? categoryId = null, string? searchTerm = null);
        Task<ApiResponse<BusinessDto?>> GetBusinessByIdAsync(int id);
        Task<ApiResponse<List<ServiceDto>>> GetServicesByBusinessAsync(int businessId);
        Task<ApiResponse<List<EmployeeDto>>> GetEmployeesByBusinessAsync(int businessId);
        Task<ApiResponse<List<AppointmentDto>>> GetAppointmentsByBusinessAsync(int businessId);
        Task<ApiResponse<BusinessDto>> CreateBusinessAsync(CreateBusinessDto createBusinessDto);
        Task<ApiResponse<BusinessDto?>> UpdateBusinessAsync(int id, CreateBusinessDto updateBusinessDto);
        Task<ApiResponse<bool>> DeleteBusinessAsync(int id);
    }
}
