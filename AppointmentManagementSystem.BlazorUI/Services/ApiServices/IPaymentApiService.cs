using AppointmentManagementSystem.Application.DTOs;
using AppointmentManagementSystem.BlazorUI.Models;

namespace AppointmentManagementSystem.BlazorUI.Services.ApiServices
{
    public interface IPaymentApiService
    {
        Task<ApiResponse<CardRegistrationResponseDto>> InitiateCardRegistrationAsync(InitiateCardRegistrationDto request);
        Task<ApiResponse<DirectCardRegistrationResponseDto>> InitiateDirectCardRegistrationAsync(object request);
        Task<ApiResponse<ChargeManualBillingResponseDto>> InitiateManualBillingAsync(ManualBillingRequestDto request);
        Task<ApiResponse<SubscriptionDto>> GetSubscriptionAsync(int businessId);
        Task<ApiResponse<List<PaymentDto>>> GetPaymentHistoryAsync(int businessId);
        Task<ApiResponse<bool>> CompleteCardRegistrationAsync(CompleteCardRegistrationDto request);
    }

    public class DirectCardRegistrationResponseDto
    {
        public bool Success { get; set; }
        public string? MerchantOid { get; set; }
        public string? Message { get; set; }
    }

    public class ManualBillingRequestDto
    {
        public int BusinessId { get; set; }
        public int BillingYear { get; set; }
        public int BillingMonth { get; set; }
        public string Email { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string CardOwner { get; set; } = string.Empty;
        public string CardNumber { get; set; } = string.Empty;
        public string ExpiryMonth { get; set; } = string.Empty;
        public string ExpiryYear { get; set; } = string.Empty;
        public string CVV { get; set; } = string.Empty;
    }

    public class ChargeManualBillingResponseDto
    {
        public bool Success { get; set; }
        public string? MerchantOid { get; set; }
        public string? Message { get; set; }
        public string? PaymentUrl { get; set; }
    }
}