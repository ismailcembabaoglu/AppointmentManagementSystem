using AppointmentManagementSystem.Application.DTOs;
using AppointmentManagementSystem.BlazorUI.Models;

namespace AppointmentManagementSystem.BlazorUI.Services.ApiServices
{
    public interface IServiceApiService
    {
        Task<ApiResponse<List<ServiceDto>>> GetAllServicesAsync(int? businessId = null);
        Task<ApiResponse<ServiceDto?>> GetServiceByIdAsync(int id);
        Task<ApiResponse<ServiceDto>> CreateServiceAsync(CreateServiceDto createServiceDto);
        Task<ApiResponse<ServiceDto?>> UpdateServiceAsync(int id, CreateServiceDto updateServiceDto);
        Task<ApiResponse<bool>> DeleteServiceAsync(int id);
    }
}
