using AppointmentManagementSystem.Application.DTOs;
using AppointmentManagementSystem.BlazorUI.Models;
using Blazored.LocalStorage;
using System.Net.Http.Json;

namespace AppointmentManagementSystem.Maui.Services.ApiServices
{
    public class PhotoApiService : BaseApiService, IPhotoApiService
    {
        public PhotoApiService(HttpClient httpClient, ILocalStorageService localStorage)
            : base(httpClient, localStorage)
        {
        }

        public async Task<ApiResponse<PhotoDto>> UploadBusinessPhotoAsync(int businessId, UploadPhotoDto photoDto)
        {
            try
            {
                await AddAuthorizationHeader();
                var response = await _httpClient.PostAsJsonAsync($"api/photos/business/{businessId}", photoDto);
                return await HandleApiResponse<PhotoDto>(response);
            }
            catch (Exception ex)
            {
                return new ApiResponse<PhotoDto> { Success = false, Message = $"Hata: {ex.Message}" };
            }
        }

        public async Task<ApiResponse<PhotoDto>> UploadEmployeePhotoAsync(int employeeId, UploadPhotoDto photoDto)
        {
            try
            {
                await AddAuthorizationHeader();
                var response = await _httpClient.PostAsJsonAsync($"api/photos/employee/{employeeId}", photoDto);
                return await HandleApiResponse<PhotoDto>(response);
            }
            catch (Exception ex)
            {
                return new ApiResponse<PhotoDto> { Success = false, Message = $"Hata: {ex.Message}" };
            }
        }

        public async Task<ApiResponse<PhotoDto>> UploadServicePhotoAsync(int serviceId, UploadPhotoDto photoDto)
        {
            try
            {
                await AddAuthorizationHeader();
                var response = await _httpClient.PostAsJsonAsync($"api/photos/service/{serviceId}", photoDto);
                return await HandleApiResponse<PhotoDto>(response);
            }
            catch (Exception ex)
            {
                return new ApiResponse<PhotoDto> { Success = false, Message = $"Hata: {ex.Message}" };
            }
        }

        public async Task<ApiResponse<bool>> DeletePhotoAsync(int photoId)
        {
            try
            {
                await AddAuthorizationHeader();
                var response = await _httpClient.DeleteAsync($"api/photos/{photoId}");
                return await HandleApiResponse<bool>(response);
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool> { Success = false, Message = $"Hata: {ex.Message}" };
            }
        }
    }
}
