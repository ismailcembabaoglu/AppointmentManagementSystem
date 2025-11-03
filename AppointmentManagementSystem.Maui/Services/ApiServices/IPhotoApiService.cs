using AppointmentManagementSystem.Application.DTOs;
using AppointmentManagementSystem.BlazorUI.Models;

namespace AppointmentManagementSystem.BlazorUI.Services.ApiServices
{
    public interface IPhotoApiService
    {
        Task<ApiResponse<PhotoDto>> UploadBusinessPhotoAsync(int businessId, UploadPhotoDto photoDto);
        Task<ApiResponse<PhotoDto>> UploadEmployeePhotoAsync(int employeeId, UploadPhotoDto photoDto);
        Task<ApiResponse<PhotoDto>> UploadServicePhotoAsync(int serviceId, UploadPhotoDto photoDto);
        Task<ApiResponse<bool>> DeletePhotoAsync(int photoId);
    }
}
