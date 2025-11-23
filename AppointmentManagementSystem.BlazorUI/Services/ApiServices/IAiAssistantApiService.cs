using AppointmentManagementSystem.Application.DTOs.Ai;
using AppointmentManagementSystem.BlazorUI.Models;

namespace AppointmentManagementSystem.BlazorUI.Services.ApiServices
{
    public interface IAiAssistantApiService
    {
        Task<ApiResponse<AiRecommendationResponseDto>> GetRecommendationsAsync(AiRecommendationRequestDto requestDto);
        Task<ApiResponse<AiBusinessInsightResponseDto>> GetBusinessInsightsAsync(AiBusinessInsightRequestDto requestDto);
        Task<ApiResponse<AiBusinessReportFileDto>> DownloadBusinessReportAsync();
    }
}
