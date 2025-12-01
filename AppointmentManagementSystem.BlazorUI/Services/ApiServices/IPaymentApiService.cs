using AppointmentManagementSystem.Application.DTOs;
using AppointmentManagementSystem.BlazorUI.Models;

namespace AppointmentManagementSystem.BlazorUI.Services.ApiServices
{
    public interface IPaymentApiService
    {
        Task<ApiResponse<CardRegistrationResponseDto>> InitiateCardRegistrationAsync(InitiateCardRegistrationDto request);
        Task<ApiResponse<DirectCardRegistrationResponseDto>> InitiateDirectCardRegistrationAsync(object request);
        Task<ApiResponse<SubscriptionDto>> GetSubscriptionAsync(int businessId);
        Task<ApiResponse<List<PaymentDto>>> GetPaymentHistoryAsync(int businessId);
        Task<ApiResponse<bool>> CompleteCardRegistrationAsync(CompleteCardRegistrationDto request);
    }

    public class DirectCardRegistrationResponseDto
    {
        public bool Success { get; set; }
        public string? MerchantOid { get; set; }
        public string? Message { get; set; }
        public string? PayTRUserToken { get; set; }
        public string? PayTRCardToken { get; set; }
        public string? MaskedCardNumber { get; set; }
        public string? CardBrand { get; set; }
    }
}