using AppointmentManagementSystem.Maui.Models;
using Blazored.LocalStorage;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace AppointmentManagementSystem.Maui.Services.ApiServices
{
    public class BaseApiService
    {
        protected readonly HttpClient _httpClient;
        protected readonly ILocalStorageService _localStorage;

        public BaseApiService(HttpClient httpClient, ILocalStorageService localStorage)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
        }

        protected async Task AddAuthorizationHeader()
        {
            var token = await _localStorage.GetItemAsStringAsync("authToken");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token.Replace("\"", ""));
            }
        }

        protected async Task<ApiResponse<T>> HandleApiResponse<T>(HttpResponseMessage response)
        {
            try
            {
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ApiResponse<T>>();
                    return result ?? new ApiResponse<T> { Success = false, Message = "Bilinmeyen hata" };
                }
                else
                {
                    var errorResult = await response.Content.ReadFromJsonAsync<ApiResponse<T>>();
                    return errorResult ?? new ApiResponse<T>
                    {
                        Success = false,
                        Message = $"HTTP {(int)response.StatusCode}: {response.ReasonPhrase}"
                    };
                }
            }
            catch (Exception ex)
            {
                return new ApiResponse<T> { Success = false, Message = $"Hata: {ex.Message}" };
            }
        }
    }
}
