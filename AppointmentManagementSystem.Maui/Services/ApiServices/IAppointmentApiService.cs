using AppointmentManagementSystem.Application.DTOs;
using AppointmentManagementSystem.Maui.Models;

namespace AppointmentManagementSystem.Maui.Services.ApiServices
{
    public interface IAppointmentApiService
    {
        Task<ApiResponse<List<AppointmentDto>>> GetAllAppointmentsAsync(int? customerId = null, int? businessId = null);
        Task<ApiResponse<AppointmentDto?>> GetAppointmentByIdAsync(int id);
        Task<ApiResponse<AppointmentDto>> CreateAppointmentAsync(CreateAppointmentDto createAppointmentDto);
        Task<ApiResponse<AppointmentDto?>> UpdateAppointmentStatusAsync(int id, string status);
        Task<ApiResponse<AppointmentDto?>> AddAppointmentRatingAsync(int id, AddAppointmentRatingDto ratingDto);
        Task<ApiResponse<bool>> DeleteAppointmentAsync(int id);
    }
}
