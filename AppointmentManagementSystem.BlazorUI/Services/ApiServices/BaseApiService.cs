using AppointmentManagementSystem.BlazorUI.Models;
using Blazored.LocalStorage;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace AppointmentManagementSystem.BlazorUI.Services.ApiServices
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

        protected async Task<HttpRequestMessage> CreateRequestWithAuth(HttpMethod method, string requestUri)
        {
            var request = new HttpRequestMessage(method, requestUri);
            
            var token = await _localStorage.GetItemAsStringAsync("authToken");
            if (!string.IsNullOrEmpty(token))
            {
                token = token.Trim('"');
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            
            return request;
        }

        protected async Task AddAuthorizationHeader()
        {
            // Bu metod artık kullanılmıyor ama geriye dönük uyumluluk için bırakıldı
            // Yeni yaklaşım: Her istekte CreateRequestWithAuth kullan
            await Task.CompletedTask;
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
                    // Hata durumunda detaylı bilgi ver
                    string errorMessage;
                    try
                    {
                        var errorResult = await response.Content.ReadFromJsonAsync<ApiResponse<T>>();
                        errorMessage = errorResult?.Message ?? $"HTTP {(int)response.StatusCode}: {response.ReasonPhrase}";
                    }
                    catch
                    {
                        errorMessage = $"HTTP {(int)response.StatusCode}: {response.ReasonPhrase}";
                    }

                    return new ApiResponse<T>
                    {
                        Success = false,
                        Message = errorMessage
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
