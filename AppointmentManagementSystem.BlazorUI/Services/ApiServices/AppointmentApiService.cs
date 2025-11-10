using AppointmentManagementSystem.Application.DTOs;
using AppointmentManagementSystem.BlazorUI.Models;
using Blazored.LocalStorage;
using System.Net.Http.Json;

namespace AppointmentManagementSystem.BlazorUI.Services.ApiServices
{
    public class AppointmentApiService : BaseApiService, IAppointmentApiService
    {
        public AppointmentApiService(HttpClient httpClient, ILocalStorageService localStorage)
            : base(httpClient, localStorage)
        {
        }

        public async Task<ApiResponse<List<AppointmentDto>>> GetAllAppointmentsAsync(int? customerId = null, int? businessId = null)
        {
            try
            {
                var queryString = "";
                if (customerId.HasValue)
                    queryString += $"?customerId={customerId.Value}";
                if (businessId.HasValue)
                    queryString += string.IsNullOrEmpty(queryString) ? $"?businessId={businessId.Value}" : $"&businessId={businessId.Value}";

                var request = await CreateRequestWithAuth(HttpMethod.Get, $"api/appointments{queryString}");
                var response = await _httpClient.SendAsync(request);
                return await HandleApiResponse<List<AppointmentDto>>(response);
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<AppointmentDto>> { Success = false, Message = $"Hata: {ex.Message}" };
            }
        }

        public async Task<ApiResponse<AppointmentDto?>> GetAppointmentByIdAsync(int id)
        {
            try
            {
                var request = await CreateRequestWithAuth(HttpMethod.Get, $"api/appointments/{id}");
                var response = await _httpClient.SendAsync(request);
                return await HandleApiResponse<AppointmentDto?>(response);
            }
            catch (Exception ex)
            {
                return new ApiResponse<AppointmentDto?> { Success = false, Message = $"Hata: {ex.Message}" };
            }
        }

        public async Task<ApiResponse<AppointmentDto>> CreateAppointmentAsync(CreateAppointmentDto createAppointmentDto)
        {
            try
            {
                var request = await CreateRequestWithAuth(HttpMethod.Post, "api/appointments");
                request.Content = JsonContent.Create(createAppointmentDto);
                var response = await _httpClient.SendAsync(request);
                return await HandleApiResponse<AppointmentDto>(response);
            }
            catch (Exception ex)
            {
                return new ApiResponse<AppointmentDto> { Success = false, Message = $"Hata: {ex.Message}" };
            }
        }

        public async Task<ApiResponse<AppointmentDto?>> UpdateAppointmentStatusAsync(int id, string status)
        {
            try
            {
                var updateStatusDto = new { Status = status };
                var request = await CreateRequestWithAuth(HttpMethod.Put, $"api/appointments/{id}/status");
                request.Content = JsonContent.Create(updateStatusDto);
                var response = await _httpClient.SendAsync(request);
                return await HandleApiResponse<AppointmentDto?>(response);
            }
            catch (Exception ex)
            {
                return new ApiResponse<AppointmentDto?> { Success = false, Message = $"Hata: {ex.Message}" };
            }
        }

        public async Task<ApiResponse<AppointmentDto?>> AddAppointmentRatingAsync(int id, AddAppointmentRatingDto ratingDto)
        {
            try
            {
                var request = await CreateRequestWithAuth(HttpMethod.Put, $"api/appointments/{id}/rating");
                request.Content = JsonContent.Create(ratingDto);
                var response = await _httpClient.SendAsync(request);
                return await HandleApiResponse<AppointmentDto?>(response);
            }
            catch (Exception ex)
            {
                return new ApiResponse<AppointmentDto?> { Success = false, Message = $"Hata: {ex.Message}" };
            }
        }

        public async Task<ApiResponse<bool>> DeleteAppointmentAsync(int id)
        {
            try
            {
                var request = await CreateRequestWithAuth(HttpMethod.Delete, $"api/appointments/{id}");
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
