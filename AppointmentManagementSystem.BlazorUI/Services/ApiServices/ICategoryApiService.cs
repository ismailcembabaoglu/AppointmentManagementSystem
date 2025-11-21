using AppointmentManagementSystem.Application.DTOs;
using AppointmentManagementSystem.BlazorUI.Models;

namespace AppointmentManagementSystem.BlazorUI.Services.ApiServices
{
    public interface ICategoryApiService
    {
        Task<ApiResponse<List<CategoryDto>>> GetAllCategoriesAsync(int pageNumber = 1, int pageSize = 10);
        Task<ApiResponse<CategoryDto?>> GetCategoryByIdAsync(int id);
        Task<ApiResponse<List<BusinessDto>>> GetBusinessesByCategoryAsync(int categoryId, int pageNumber = 1, int pageSize = 10);
    }
}
