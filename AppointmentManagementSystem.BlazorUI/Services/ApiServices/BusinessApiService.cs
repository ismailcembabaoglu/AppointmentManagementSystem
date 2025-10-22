using AppointmentManagementSystem.Application.DTOs;
using AppointmentManagementSystem.BlazorUI.Models;
using Blazored.LocalStorage;
using System.Net.Http.Json;

namespace AppointmentManagementSystem.BlazorUI.Services.ApiServices
{
    public class BusinessApiService : BaseApiService, IBusinessApiService
    {
        public BusinessApiService(HttpClient httpClient, ILocalStorageService localStorage)
            : base(httpClient, localStorage)
        {
        }

        public async Task<ApiResponse<List<BusinessDto>>> GetAllBusinessesAsync(int? categoryId = null, string? searchTerm = null)
        {
            try
            {
                var queryString = "";
                if (categoryId.HasValue)
                    queryString += $"?categoryId={categoryId.Value}";
                if (!string.IsNullOrEmpty(searchTerm))
                    queryString += string.IsNullOrEmpty(queryString) ? $"?search={searchTerm}" : $"&search={searchTerm}";

                var response = await _httpClient.GetAsync($"api/businesses{queryString}");
                return await HandleApiResponse<List<BusinessDto>>(response);
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<BusinessDto>> { Success = false, Message = $"Hata: {ex.Message}" };
            }
        }

        public async Task<ApiResponse<BusinessDto?>> GetBusinessByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/businesses/{id}");
                return await HandleApiResponse<BusinessDto?>(response);
            }
            catch (Exception ex)
            {
                return new ApiResponse<BusinessDto?> { Success = false, Message = $"Hata: {ex.Message}" };
            }
        }

        public async Task<ApiResponse<List<ServiceDto>>> GetServicesByBusinessAsync(int businessId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/businesses/{businessId}/services");
                return await HandleApiResponse<List<ServiceDto>>(response);
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<ServiceDto>> { Success = false, Message = $"Hata: {ex.Message}" };
            }
        }

        public async Task<ApiResponse<List<EmployeeDto>>> GetEmployeesByBusinessAsync(int businessId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/businesses/{businessId}/employees");
                return await HandleApiResponse<List<EmployeeDto>>(response);
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<EmployeeDto>> { Success = false, Message = $"Hata: {ex.Message}" };
            }
        }

        public async Task<ApiResponse<List<AppointmentDto>>> GetAppointmentsByBusinessAsync(int businessId)
        {
            try
            {
                await AddAuthorizationHeader();
                var response = await _httpClient.GetAsync($"api/businesses/{businessId}/appointments");
                return await HandleApiResponse<List<AppointmentDto>>(response);
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<AppointmentDto>> { Success = false, Message = $"Hata: {ex.Message}" };
            }
        }

        public async Task<ApiResponse<BusinessDto>> CreateBusinessAsync(CreateBusinessDto createBusinessDto)
        {
            try
            {
                await AddAuthorizationHeader();
                var response = await _httpClient.PostAsJsonAsync("api/businesses", createBusinessDto);
                return await HandleApiResponse<BusinessDto>(response);
            }
            catch (Exception ex)
            {
                return new ApiResponse<BusinessDto> { Success = false, Message = $"Hata: {ex.Message}" };
            }
        }

        public async Task<ApiResponse<BusinessDto?>> UpdateBusinessAsync(int id, CreateBusinessDto updateBusinessDto)
        {
            try
            {
                await AddAuthorizationHeader();
                var response = await _httpClient.PutAsJsonAsync($"api/businesses/{id}", updateBusinessDto);
                return await HandleApiResponse<BusinessDto?>(response);
            }
            catch (Exception ex)
            {
                return new ApiResponse<BusinessDto?> { Success = false, Message = $"Hata: {ex.Message}" };
            }
        }

        public async Task<ApiResponse<bool>> DeleteBusinessAsync(int id)
        {
            try
            {
                await AddAuthorizationHeader();
                var response = await _httpClient.DeleteAsync($"api/businesses/{id}");
                return await HandleApiResponse<bool>(response);
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool> { Success = false, Message = $"Hata: {ex.Message}" };
            }
        }
    }
}
