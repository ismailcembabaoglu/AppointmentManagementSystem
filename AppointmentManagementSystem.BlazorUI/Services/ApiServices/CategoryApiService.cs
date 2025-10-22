using AppointmentManagementSystem.Application.DTOs;
using AppointmentManagementSystem.BlazorUI.Models;
using Blazored.LocalStorage;
using System.Net.Http.Json;

namespace AppointmentManagementSystem.BlazorUI.Services.ApiServices
{
    public class CategoryApiService : BaseApiService, ICategoryApiService
    {
        public CategoryApiService(HttpClient httpClient, ILocalStorageService localStorage)
            : base(httpClient, localStorage)
        {
        }

        public async Task<ApiResponse<List<CategoryDto>>> GetAllCategoriesAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/categories");
                return await HandleApiResponse<List<Application.DTOs.CategoryDto>>(response);
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<CategoryDto>> { Success = false, Message = $"Hata: {ex.Message}" };
            }
        }

        public async Task<ApiResponse<CategoryDto?>> GetCategoryByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/categories/{id}");
                return await HandleApiResponse<Application.DTOs.CategoryDto?>(response);
            }
            catch (Exception ex)
            {
                return new ApiResponse<CategoryDto?> { Success = false, Message = $"Hata: {ex.Message}" };
            }
        }

        public async Task<ApiResponse<List<BusinessDto>>> GetBusinessesByCategoryAsync(int categoryId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/categories/{categoryId}/businesses");
                return await HandleApiResponse<List<Application.DTOs.BusinessDto>>(response);
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<BusinessDto>> { Success = false, Message = $"Hata: {ex.Message}" };
            }
        }

     
    }
}
