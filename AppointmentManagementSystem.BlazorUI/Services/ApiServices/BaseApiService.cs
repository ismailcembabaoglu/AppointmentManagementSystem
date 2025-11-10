using AppointmentManagementSystem.BlazorUI.Models;
using Blazored.LocalStorage;
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

        // AddAuthorizationHeader artık gerekli değil - AuthorizationMessageHandler otomatik ekliyor
        // Geriye dönük uyumluluk için boş metod bırakıldı
        [Obsolete("Authorization header artık otomatik ekleniyor. Bu metodu çağırmaya gerek yok.")]
        protected Task AddAuthorizationHeader()
        {
            return Task.CompletedTask;
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
