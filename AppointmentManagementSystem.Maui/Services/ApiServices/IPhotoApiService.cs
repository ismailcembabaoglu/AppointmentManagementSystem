using AppointmentManagementSystem.Application.DTOs;
using AppointmentManagementSystem.Maui.Models;

namespace AppointmentManagementSystem.Maui.Services.ApiServices
{
    public interface IPhotoApiService
    {
        Task<ApiResponse<PhotoDto>> UploadBusinessPhotoAsync(int businessId, UploadPhotoDto photoDto);
        Task<ApiResponse<PhotoDto>> UploadEmployeePhotoAsync(int employeeId, UploadPhotoDto photoDto);
        Task<ApiResponse<PhotoDto>> UploadServicePhotoAsync(int serviceId, UploadPhotoDto photoDto);
        Task<ApiResponse<bool>> DeletePhotoAsync(int photoId);
    }
}
