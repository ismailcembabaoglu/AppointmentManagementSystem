using AppointmentManagementSystem.Application.DTOs;
using AppointmentManagementSystem.BlazorUI.Models;

namespace AppointmentManagementSystem.BlazorUI.Services.ApiServices
{
    public interface IAppointmentApiService
    {
        Task<ApiResponse<PaginatedResult<AppointmentDto>>> GetAllAppointmentsAsync(
            int? customerId = null,
            int? businessId = null,
            string? status = null,
            string? sortBy = null,
            int pageNumber = 1,
            int pageSize = 10);
        Task<ApiResponse<AppointmentDto?>> GetAppointmentByIdAsync(int id);
        Task<ApiResponse<AppointmentDto>> CreateAppointmentAsync(CreateAppointmentDto createAppointmentDto);
        Task<ApiResponse<AppointmentDto?>> UpdateAppointmentStatusAsync(int id, string status);
        Task<ApiResponse<AppointmentDto?>> AddAppointmentRatingAsync(int id, AddAppointmentRatingDto ratingDto);
        Task<ApiResponse<bool>> DeleteAppointmentAsync(int id);
        Task<ApiResponse<List<string>>> GetAppointmentPhotosAsync(int appointmentId); // Yeni

    }
}
