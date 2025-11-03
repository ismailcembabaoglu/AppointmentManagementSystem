using AppointmentManagementSystem.Application.DTOs;
using AppointmentManagementSystem.BlazorUI.Models;
using Blazored.LocalStorage;
using System.Net.Http.Json;

namespace AppointmentManagementSystem.Maui.Services.ApiServices
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

                await AddAuthorizationHeader();
                var response = await _httpClient.GetAsync($"api/appointments{queryString}");
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
                await AddAuthorizationHeader();
                var response = await _httpClient.GetAsync($"api/appointments/{id}");
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
                await AddAuthorizationHeader();
                var response = await _httpClient.PostAsJsonAsync("api/appointments", createAppointmentDto);
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
                await AddAuthorizationHeader();
                var updateStatusDto = new { Status = status };
                var response = await _httpClient.PutAsJsonAsync($"api/appointments/{id}/status", updateStatusDto);
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
                await AddAuthorizationHeader();
                var response = await _httpClient.PutAsJsonAsync($"api/appointments/{id}/rating", ratingDto);
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
                await AddAuthorizationHeader();
                var response = await _httpClient.DeleteAsync($"api/appointments/{id}");
                return await HandleApiResponse<bool>(response);
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool> { Success = false, Message = $"Hata: {ex.Message}" };
            }
        }
    }
}
