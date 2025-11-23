using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AppointmentManagementSystem.Application.DTOs.Ai;
using AppointmentManagementSystem.BlazorUI.Models;
using Blazored.LocalStorage;

namespace AppointmentManagementSystem.BlazorUI.Services.ApiServices
{
    public class AiAssistantApiService : BaseApiService, IAiAssistantApiService
    {
        public AiAssistantApiService(HttpClient httpClient, ILocalStorageService localStorage)
            : base(httpClient, localStorage)
        {
        }

        public async Task<ApiResponse<AiRecommendationResponseDto>> GetRecommendationsAsync(AiRecommendationRequestDto requestDto)
        {
            try
            {
                var request = await CreateRequestWithAuth(HttpMethod.Post, "api/ai/recommendations");
                request.Content = JsonContent.Create(requestDto);
                var response = await _httpClient.SendAsync(request);
                return await HandleApiResponse<AiRecommendationResponseDto>(response);
            }
            catch (Exception ex)
            {
                return new ApiResponse<AiRecommendationResponseDto>
                {
                    Success = false,
                    Message = $"Hata: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse<AiBusinessInsightResponseDto>> GetBusinessInsightsAsync(AiBusinessInsightRequestDto requestDto)
        {
            try
            {
                var request = await CreateRequestWithAuth(HttpMethod.Post, "api/ai/business/insights");
                request.Content = JsonContent.Create(requestDto);
                var response = await _httpClient.SendAsync(request);
                return await HandleApiResponse<AiBusinessInsightResponseDto>(response);
            }
            catch (Exception ex)
            {
                return new ApiResponse<AiBusinessInsightResponseDto>
                {
                    Success = false,
                    Message = $"Hata: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse<AiBusinessReportFileDto>> DownloadBusinessReportAsync()
        {
            try
            {
                var request = await CreateRequestWithAuth(HttpMethod.Get, "api/ai/business/report");
                var response = await _httpClient.SendAsync(request);
                return await HandleApiResponse<AiBusinessReportFileDto>(response);
            }
            catch (Exception ex)
            {
                return new ApiResponse<AiBusinessReportFileDto>
                {
                    Success = false,
                    Message = $"Hata: {ex.Message}"
                };
            }
        }
    }
}
