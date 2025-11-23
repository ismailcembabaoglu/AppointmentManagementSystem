using AppointmentManagementSystem.Application.DTOs;
using AppointmentManagementSystem.BlazorUI.Models;

namespace AppointmentManagementSystem.BlazorUI.Services.ApiServices
{
    public interface IPaymentApiService
    {
        Task<ApiResponse<CardRegistrationResponseDto>> InitiateCardRegistrationAsync(InitiateCardRegistrationDto request);
        Task<ApiResponse<SubscriptionDto>> GetSubscriptionAsync(int businessId);
        Task<ApiResponse<List<PaymentDto>>> GetPaymentHistoryAsync(int businessId);
        Task<ApiResponse<bool>> CompleteCardRegistrationAsync(CompleteCardRegistrationDto request);
    }
}