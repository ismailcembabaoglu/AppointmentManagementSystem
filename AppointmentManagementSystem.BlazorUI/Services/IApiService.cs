using AppointmentManagementSystem.BlazorUI.Models;

namespace AppointmentManagementSystem.BlazorUI.Services
{
    public interface IApiService
    {
        Task<ApiResponse<AuthResponseDto>> LoginAsync(LoginDto loginDto);
        Task<ApiResponse<AuthResponseDto>> RegisterAsync(RegisterDto registerDto);
    }
}
