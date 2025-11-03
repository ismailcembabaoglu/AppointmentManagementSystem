using AppointmentManagementSystem.Application.DTOs;
using AppointmentManagementSystem.Maui.Models;

namespace AppointmentManagementSystem.Maui.Services
{
    public interface IApiService
    {
        Task<ApiResponse<AuthResponseDto>> LoginAsync(LoginDto loginDto);
        Task<ApiResponse<AuthResponseDto>> RegisterAsync(RegisterDto registerDto);
        Task<ApiResponse<AuthResponseDto>> RegisterBusinessAsync(RegisterBusinessDto registerBusinessDto);
    }
}
