using AppointmentManagementSystem.Application.DTOs;
using AppointmentManagementSystem.Maui.Models;
using Blazored.LocalStorage;
using System.Net.Http.Json;

namespace AppointmentManagementSystem.Maui.Services.ApiServices
{
    public class ServiceApiService : BaseApiService, IServiceApiService
    {
        public ServiceApiService(HttpClient httpClient, ILocalStorageService localStorage)
            : base(httpClient, localStorage)
        {
        }

        public async Task<ApiResponse<List<ServiceDto>>> GetAllServicesAsync(int? businessId = null)
        {
            try
            {
                var queryString = businessId.HasValue ? $"?businessId={businessId.Value}" : "";
                var response = await _httpClient.GetAsync($"api/services{queryString}");
                return await HandleApiResponse<List<ServiceDto>>(response);
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<ServiceDto>> { Success = false, Message = $"Hata: {ex.Message}" };
            }
        }

        public async Task<ApiResponse<ServiceDto?>> GetServiceByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/services/{id}");
                return await HandleApiResponse<ServiceDto?>(response);
            }
            catch (Exception ex)
            {
                return new ApiResponse<ServiceDto?> { Success = false, Message = $"Hata: {ex.Message}" };
            }
        }

        public async Task<ApiResponse<ServiceDto>> CreateServiceAsync(CreateServiceDto createServiceDto)
        {
            try
            {
                await AddAuthorizationHeader();
                var response = await _httpClient.PostAsJsonAsync("api/services", createServiceDto);
                return await HandleApiResponse<ServiceDto>(response);
            }
            catch (Exception ex)
            {
                return new ApiResponse<ServiceDto> { Success = false, Message = $"Hata: {ex.Message}" };
            }
        }

        public async Task<ApiResponse<ServiceDto?>> UpdateServiceAsync(int id, CreateServiceDto updateServiceDto)
        {
            try
            {
                await AddAuthorizationHeader();
                var response = await _httpClient.PutAsJsonAsync($"api/services/{id}", updateServiceDto);
                return await HandleApiResponse<ServiceDto?>(response);
            }
            catch (Exception ex)
            {
                return new ApiResponse<ServiceDto?> { Success = false, Message = $"Hata: {ex.Message}" };
            }
        }

        public async Task<ApiResponse<bool>> DeleteServiceAsync(int id)
        {
            try
            {
                await AddAuthorizationHeader();
                var response = await _httpClient.DeleteAsync($"api/services/{id}");
                return await HandleApiResponse<bool>(response);
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool> { Success = false, Message = $"Hata: {ex.Message}" };
            }
        }
    }
}
