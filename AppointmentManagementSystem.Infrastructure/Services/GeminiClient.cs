using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AppointmentManagementSystem.Domain.Interfaces;
using Microsoft.Extensions.Options;

namespace AppointmentManagementSystem.Infrastructure.Services
{
    public class GeminiClient : IGeminiClient
    {
        private readonly HttpClient _httpClient;
        private readonly GeminiOptions _options;
        private readonly JsonSerializerOptions _serializerOptions = new(JsonSerializerDefaults.Web);

        public GeminiClient(HttpClient httpClient, IOptions<GeminiOptions> options)
        {
            _httpClient = httpClient;
            _options = options.Value;
        }

        public async Task<string> GenerateContentAsync(string prompt, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(_options.ApiKey))
            {
                return "Gemini API anahtarı yapılandırılmamış görünüyor.";
            }

            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        role = "user",
                        parts = new[]
                        {
                            new { text = prompt }
                        }
                    }
                }
            };

            var requestUri = $"{_options.BaseUrl.TrimEnd('/')}/models/{_options.Model}:generateContent?key={_options.ApiKey}";

            try
            {
                using var content = new StringContent(JsonSerializer.Serialize(requestBody, _serializerOptions), Encoding.UTF8, "application/json");
                using var response = await _httpClient.PostAsync(requestUri, content, cancellationToken);
                response.EnsureSuccessStatusCode();

                await using var responseStream = await response.Content.ReadAsStreamAsync(cancellationToken);
                var geminiResponse = await JsonSerializer.DeserializeAsync<GeminiResponse>(responseStream, _serializerOptions, cancellationToken);

                return geminiResponse?.Candidates?.FirstOrDefault()?.Content?.Parts?.FirstOrDefault()?.Text
                    ?? "Gemini servisinden yanıt alınamadı.";
            }
            catch (Exception ex)
            {
                return $"Yapay zeka yanıtı alınırken bir hata oluştu: {ex.Message}";
            }
        }

        private class GeminiResponse
        {
            public List<GeminiCandidate>? Candidates { get; set; }
        }

        private class GeminiCandidate
        {
            public GeminiContent? Content { get; set; }
        }

        private class GeminiContent
        {
            public List<GeminiPart>? Parts { get; set; }
        }

        private class GeminiPart
        {
            public string? Text { get; set; }
        }
    }
}
