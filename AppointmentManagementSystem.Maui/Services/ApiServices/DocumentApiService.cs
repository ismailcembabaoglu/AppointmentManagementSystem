using AppointmentManagementSystem.Application.DTOs;
using AppointmentManagementSystem.BlazorUI.Models;
using Blazored.LocalStorage;
using System.Net.Http.Json;

namespace AppointmentManagementSystem.Maui.Services.ApiServices
{
    public class DocumentApiService : BaseApiService, IDocumentApiService
    {
        public DocumentApiService(HttpClient httpClient, ILocalStorageService localStorage)
            : base(httpClient, localStorage)
        {
        }

        public async Task<ApiResponse<DocumentDto>> UploadEmployeeDocumentAsync(int employeeId, UploadDocumentDto documentDto)
        {
            try
            {
                await AddAuthorizationHeader();
                var response = await _httpClient.PostAsJsonAsync($"api/documents/employee/{employeeId}", documentDto);
                return await HandleApiResponse<DocumentDto>(response);
            }
            catch (Exception ex)
            {
                return new ApiResponse<DocumentDto> { Success = false, Message = $"Hata: {ex.Message}" };
            }
        }

        public async Task<ApiResponse<bool>> DeleteDocumentAsync(int documentId)
        {
            try
            {
                await AddAuthorizationHeader();
                var response = await _httpClient.DeleteAsync($"api/documents/{documentId}");
                return await HandleApiResponse<bool>(response);
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool> { Success = false, Message = $"Hata: {ex.Message}" };
            }
        }
    }
}
