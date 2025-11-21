using AppointmentManagementSystem.Application.DTOs;
using AppointmentManagementSystem.BlazorUI.Models;

namespace AppointmentManagementSystem.BlazorUI.Services.ApiServices
{
    public interface ICategoryApiService
    {
        Task<ApiResponse<PaginatedResult<CategoryDto>>> GetAllCategoriesAsync(int pageNumber = 1, int pageSize = 10);
        Task<ApiResponse<CategoryDto?>> GetCategoryByIdAsync(int id);
        Task<ApiResponse<PaginatedResult<BusinessDto>>> GetBusinessesByCategoryAsync(int categoryId, int pageNumber = 1, int pageSize = 10);
    }
}
