using AppointmentManagementSystem.Application.DTOs;
using AppointmentManagementSystem.BlazorUI.Models;
using Blazored.LocalStorage;
using System.Net.Http.Json;

namespace AppointmentManagementSystem.BlazorUI.Services.ApiServices
{
    public class PaymentApiService : BaseApiService, IPaymentApiService
    {
        public PaymentApiService(HttpClient httpClient, ILocalStorageService localStorage)
            : base(httpClient, localStorage)
        {
        }

        public async Task<ApiResponse<CardRegistrationResponseDto>> InitiateCardRegistrationAsync(InitiateCardRegistrationDto request)
        {
            try
            {
                await AddAuthorizationHeader();
                var response = await _httpClient.PostAsJsonAsync("api/payments/initiate-card-registration", request);
                return await HandleApiResponse<CardRegistrationResponseDto>(response);
            }
            catch (Exception ex)
            {
                return new ApiResponse<CardRegistrationResponseDto>
                {
                    Success = false,
                    Message = $"Kart kaydı başlatılamadı: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse<SubscriptionDto>> GetSubscriptionAsync(int businessId)
        {
            try
            {
                await AddAuthorizationHeader();
                var response = await _httpClient.GetAsync($"api/payments/subscription/{businessId}");
                return await HandleApiResponse<SubscriptionDto>(response);
            }
            catch (Exception ex)
            {
                return new ApiResponse<SubscriptionDto>
                {
                    Success = false,
                    Message = $"Abonelik bilgisi alınamadı: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse<List<PaymentDto>>> GetPaymentHistoryAsync(int businessId)
        {
            try
            {
                await AddAuthorizationHeader();
                var response = await _httpClient.GetAsync($"api/payments/history/{businessId}");
                return await HandleApiResponse<List<PaymentDto>>(response);
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<PaymentDto>>
                {
                    Success = false,
                    Message = $"Ödeme geçmişi alınamadı: {ex.Message}"
                };
            }
        }
    }
}