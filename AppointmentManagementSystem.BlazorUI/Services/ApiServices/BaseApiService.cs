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
                var content = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                if (response.IsSuccessStatusCode)
                {
                    ApiResponse<T>? apiResponse = null;

                    if (!string.IsNullOrWhiteSpace(content))
                    {
                        try
                        {
                            // Öncelikle ApiResponse<T> olarak çözümlemeyi dene
                            apiResponse = JsonSerializer.Deserialize<ApiResponse<T>>(content, options);
                        }
                        catch (JsonException)
                        {
                            // İçerik doğrudan T tipi olabilir; bu durumda ham veriye geçilecek
                            apiResponse = null;
                        }
                    }

                    if (apiResponse != null)
                    {
                        apiResponse.Success |= response.IsSuccessStatusCode;
                        return apiResponse;
                    }

                    // Gelen veri ApiResponse değilse doğrudan T tipine deserialize et
                    if (!string.IsNullOrWhiteSpace(content))
                    {
                        var rawData = JsonSerializer.Deserialize<T>(content, options);
                        return new ApiResponse<T>
                        {
                            Success = true,
                            Data = rawData
                        };
                    }

                    // İçerik boş ama istek başarılı; sadece başarı bilgisini dön
                    return new ApiResponse<T>
                    {
                        Success = true
                    };
                }

                // Hata durumunda detaylı bilgi ver
                string errorMessage;
                try
                {
                    var errorResult = !string.IsNullOrWhiteSpace(content)
                        ? JsonSerializer.Deserialize<ApiResponse<T>>(content, options)
                        : null;
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
            catch (Exception ex)
            {
                return new ApiResponse<T> { Success = false, Message = $"Hata: {ex.Message}" };
            }
        }
    }
}
