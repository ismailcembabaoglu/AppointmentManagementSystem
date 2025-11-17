using AppointmentManagementSystem.Application.DTOs;
using AppointmentManagementSystem.BlazorUI.Models;

namespace AppointmentManagementSystem.BlazorUI.Services
{
    public interface IApiService
    {
        Task<ApiResponse<AuthResponseDto>> LoginAsync(LoginDto loginDto);
        Task<ApiResponse<AuthResponseDto>> RegisterAsync(RegisterDto registerDto);
        Task<ApiResponse<AuthResponseDto>> RegisterBusinessAsync(RegisterBusinessDto registerBusinessDto);
        Task<ApiResponse<string>> VerifyEmailAsync(string token);
    }
}
