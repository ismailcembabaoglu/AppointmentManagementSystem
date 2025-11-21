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

        public async Task<ApiResponse<List<CategoryDto>>> GetAllCategoriesAsync(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var request = await CreateRequestWithAuth(HttpMethod.Get, $"api/categories?pageNumber={pageNumber}&pageSize={pageSize}");
                var response = await _httpClient.SendAsync(request);
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
                var request = await CreateRequestWithAuth(HttpMethod.Get, $"api/categories/{id}");
                var response = await _httpClient.SendAsync(request);
                return await HandleApiResponse<Application.DTOs.CategoryDto?>(response);
            }
            catch (Exception ex)
            {
                return new ApiResponse<CategoryDto?> { Success = false, Message = $"Hata: {ex.Message}" };
            }
        }

        public async Task<ApiResponse<List<BusinessDto>>> GetBusinessesByCategoryAsync(int categoryId, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var request = await CreateRequestWithAuth(HttpMethod.Get, $"api/categories/{categoryId}/businesses?pageNumber={pageNumber}&pageSize={pageSize}");
                var response = await _httpClient.SendAsync(request);
                return await HandleApiResponse<List<Application.DTOs.BusinessDto>>(response);
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<BusinessDto>> { Success = false, Message = $"Hata: {ex.Message}" };
            }
        }
    }
}
