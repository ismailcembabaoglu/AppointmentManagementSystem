using AppointmentManagementSystem.Application.DTOs;
using AppointmentManagementSystem.BlazorUI.Models;

namespace AppointmentManagementSystem.BlazorUI.Services.ApiServices
{
    public interface IBusinessApiService
    {
        Task<ApiResponse<PaginatedResult<BusinessDto>>> GetAllBusinessesAsync(
            int? categoryId = null,
            string? searchTerm = null,
            string? city = null,
            string? district = null,
            double? minRating = null,
            int pageNumber = 1,
            int pageSize = 10);
        Task<ApiResponse<BusinessDto?>> GetBusinessByIdAsync(int id);
        Task<ApiResponse<List<ServiceDto>>> GetServicesByBusinessAsync(int businessId);
        Task<ApiResponse<List<EmployeeDto>>> GetEmployeesByBusinessAsync(int businessId);
        Task<ApiResponse<List<AppointmentDto>>> GetAppointmentsByBusinessAsync(int businessId);
        Task<ApiResponse<List<BusinessReviewDto>>> GetBusinessReviewsAsync(int businessId);
        Task<ApiResponse<BusinessDto>> CreateBusinessAsync(CreateBusinessDto createBusinessDto);
        Task<ApiResponse<BusinessDto?>> UpdateBusinessAsync(int id, CreateBusinessDto updateBusinessDto);
        Task<ApiResponse<bool>> DeleteBusinessAsync(int id);
    }
}
