using AppointmentManagementSystem.Application.DTOs;
using AppointmentManagementSystem.BlazorUI.Models;
using Blazored.LocalStorage;
using System.Net.Http.Json;

namespace AppointmentManagementSystem.BlazorUI.Services.ApiServices
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
                var request = await CreateRequestWithAuth(HttpMethod.Get, $"api/services{queryString}");
                var response = await _httpClient.SendAsync(request);
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
                var request = await CreateRequestWithAuth(HttpMethod.Get, $"api/services/{id}");
                var response = await _httpClient.SendAsync(request);
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
                var request = await CreateRequestWithAuth(HttpMethod.Post, "api/services");
                request.Content = JsonContent.Create(createServiceDto);
                var response = await _httpClient.SendAsync(request);
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
                var request = await CreateRequestWithAuth(HttpMethod.Put, $"api/services/{id}");
                request.Content = JsonContent.Create(updateServiceDto);
                var response = await _httpClient.SendAsync(request);
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
                var request = await CreateRequestWithAuth(HttpMethod.Delete, $"api/services/{id}");
                var response = await _httpClient.SendAsync(request);
                return await HandleApiResponse<bool>(response);
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool> { Success = false, Message = $"Hata: {ex.Message}" };
            }
        }
    }
}
