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

        public async Task<ApiResponse<CardRegistrationResponseDto>> InitiateCardRegistrationAsync(InitiateCardRegistrationDto requestDto)
        {
            try
            {
                var request = await CreateRequestWithAuth(HttpMethod.Post, "api/payments/initiate-card-registration");
                request.Content = JsonContent.Create(requestDto);
                var response = await _httpClient.SendAsync(request);
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
                var request = await CreateRequestWithAuth(HttpMethod.Get, $"api/payments/subscription/{businessId}");
                var response = await _httpClient.SendAsync(request);
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
                var request = await CreateRequestWithAuth(HttpMethod.Get, $"api/payments/history/{businessId}");
                var response = await _httpClient.SendAsync(request);
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

        public async Task<ApiResponse<bool>> CompleteCardRegistrationAsync(CompleteCardRegistrationDto requestDto)
        {
            try
            {
                var request = await CreateRequestWithAuth(HttpMethod.Post, "api/payments/complete-card-registration");
                request.Content = JsonContent.Create(requestDto);
                var response = await _httpClient.SendAsync(request);
                return await HandleApiResponse<bool>(response);
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = $"Kart kaydı tamamlanamadı: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse<DirectCardRegistrationResponseDto>> InitiateDirectCardRegistrationAsync(object requestDto)
        {
            try
            {
                Console.WriteLine("🔵 PaymentApiService: Calling Direct API endpoint...");

                // Direct API registration does not require an auth token because PayTR handles card tokenization
                var response = await _httpClient.PostAsJsonAsync("api/payments/initiate-direct-card-registration", requestDto);

                Console.WriteLine($"📥 Response Status: {response.StatusCode}");

                return await HandleApiResponse<DirectCardRegistrationResponseDto>(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Exception in PaymentApiService: {ex.Message}");
                return new ApiResponse<DirectCardRegistrationResponseDto>
                {
                    Success = false,
                    Message = $"Direct API kart kaydı başlatılamadı: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse<ChargeManualBillingResponseDto>> InitiateManualBillingAsync(ManualBillingRequestDto request)
        {
            try
            {
                // Manual billing end-point is open for 3D Secure submissions; avoid auth requirement to prevent 401s
                var response = await _httpClient.PostAsJsonAsync("api/payments/manual-billing", request);
                return await HandleApiResponse<ChargeManualBillingResponseDto>(response);
            }
            catch (Exception ex)
            {
                return new ApiResponse<ChargeManualBillingResponseDto>
                {
                    Success = false,
                    Message = $"Manual billing başlatılamadı: {ex.Message}"
                };
            }
        }
    }
}