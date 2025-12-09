using AppointmentManagementSystem.Application.DTOs;
using AppointmentManagementSystem.Maui.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using Blazored.LocalStorage;

namespace AppointmentManagementSystem.Maui.Services.ApiServices
{
    public class AppointmentApiService : BaseApiService, IAppointmentApiService
    {
        public AppointmentApiService(HttpClient httpClient, ILocalStorageService localStorage) : base(httpClient, localStorage)
        {
        }

        public async Task<ApiResponse<PaginatedResult<AppointmentDto>>> GetAllAppointmentsAsync(
            int? customerId = null,
            int? businessId = null,
            string? status = null,
            string? sortBy = null,
            int pageNumber = 1,
            int pageSize = 10)
        {
            try
            {
                var queryParams = new List<string>();

                if (customerId.HasValue)
                    queryParams.Add($"customerId={customerId.Value}");
                if (businessId.HasValue)
                    queryParams.Add($"businessId={businessId.Value}");
                if (!string.IsNullOrWhiteSpace(status))
                    queryParams.Add($"status={Uri.EscapeDataString(status)}");
                if (!string.IsNullOrWhiteSpace(sortBy))
                    queryParams.Add($"sortBy={Uri.EscapeDataString(sortBy)}");

                queryParams.Add($"pageNumber={pageNumber}");
                queryParams.Add($"pageSize={pageSize}");

                var queryString = queryParams.Any()
                    ? "?" + string.Join("&", queryParams)
                    : string.Empty;

                var request = await CreateRequestWithAuth(HttpMethod.Get, $"api/appointments{queryString}");
                var response = await _httpClient.SendAsync(request);
                return await HandleApiResponse<PaginatedResult<AppointmentDto>>(response);
            }
            catch (Exception ex)
            {
                return new ApiResponse<PaginatedResult<AppointmentDto>> { Success = false, Message = $"Hata: {ex.Message}" };
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
