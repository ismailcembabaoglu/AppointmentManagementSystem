using AppointmentManagementSystem.Application.DTOs;
using AppointmentManagementSystem.Maui.Models;
using Blazored.LocalStorage; // BU SATIRI GÜNCELLE
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace AppointmentManagementSystem.Maui.Services
{
    public class ApiService : IApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage; // Blazored.LocalStorage'den gelen interface

        public ApiService(HttpClient httpClient, ILocalStorageService localStorage)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
        }

        private async Task AddAuthorizationHeader()
        {
            var token = await _localStorage.GetItemAsStringAsync("authToken");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Replace("\"", ""));
            }
        }

        public async Task<ApiResponse<AuthResponseDto>> LoginAsync(LoginDto loginDto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/auth/login", loginDto);
                var result = await response.Content.ReadFromJsonAsync<ApiResponse<AuthResponseDto>>();
                return result ?? new ApiResponse<AuthResponseDto> { Success = false, Message = "Bilinmeyen hata" };
            }
            catch (Exception ex)
            {
                return new ApiResponse<AuthResponseDto> { Success = false, Message = $"Hata: {ex.Message}" };
            }
        }

        public async Task<ApiResponse<AuthResponseDto>> RegisterAsync(RegisterDto registerDto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/auth/register", registerDto);
                var result = await response.Content.ReadFromJsonAsync<ApiResponse<AuthResponseDto>>();
                return result ?? new ApiResponse<AuthResponseDto> { Success = false, Message = "Bilinmeyen hata" };
            }
            catch (Exception ex)
            {
                return new ApiResponse<AuthResponseDto> { Success = false, Message = $"Hata: {ex.Message}" };
            }
        }

        public async Task<ApiResponse<AuthResponseDto>> RegisterBusinessAsync(RegisterBusinessDto registerBusinessDto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/auth/register-business", registerBusinessDto);
                var result = await response.Content.ReadFromJsonAsync<ApiResponse<AuthResponseDto>>();
                return result ?? new ApiResponse<AuthResponseDto> { Success = false, Message = "Bilinmeyen hata" };
            }
            catch (Exception ex)
            {
                return new ApiResponse<AuthResponseDto> { Success = false, Message = $"Hata: {ex.Message}" };
            }
        }

        public async Task<ApiResponse<string>> GetProtectedDataAsync()
        {
            await AddAuthorizationHeader();
            try
            {
                var response = await _httpClient.GetAsync("api/protected-endpoint");
                var result = await response.Content.ReadFromJsonAsync<ApiResponse<string>>();
                return result ?? new ApiResponse<string> { Success = false, Message = "Bilinmeyen hata" };
            }
            catch (Exception ex)
            {
                return new ApiResponse<string> { Success = false, Message = $"Hata: {ex.Message}" };
            }
        }
    }
}
