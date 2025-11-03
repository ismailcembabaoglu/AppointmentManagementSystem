using AppointmentManagementSystem.Application.DTOs;
using AppointmentManagementSystem.BlazorUI.Models;

namespace AppointmentManagementSystem.Maui.Services.ApiServices
{
    public interface IDocumentApiService
    {
        Task<ApiResponse<DocumentDto>> UploadEmployeeDocumentAsync(int employeeId, UploadDocumentDto documentDto);
        Task<ApiResponse<bool>> DeleteDocumentAsync(int documentId);
    }
}
