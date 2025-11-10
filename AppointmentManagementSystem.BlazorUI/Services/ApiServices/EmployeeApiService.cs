using AppointmentManagementSystem.Application.DTOs;
using AppointmentManagementSystem.BlazorUI.Models;
using Blazored.LocalStorage;
using System.Net.Http.Json;

namespace AppointmentManagementSystem.BlazorUI.Services.ApiServices
{
    public class EmployeeApiService : BaseApiService, IEmployeeApiService
    {
        public EmployeeApiService(HttpClient httpClient, ILocalStorageService localStorage)
            : base(httpClient, localStorage)
        {
        }

        public async Task<ApiResponse<List<EmployeeDto>>> GetAllEmployeesAsync(int? businessId = null)
        {
            try
            {
                var queryString = businessId.HasValue ? $"?businessId={businessId.Value}" : "";
                var response = await _httpClient.GetAsync($"api/employees{queryString}");
                return await HandleApiResponse<List<EmployeeDto>>(response);
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<EmployeeDto>> { Success = false, Message = $"Hata: {ex.Message}" };
            }
        }

        public async Task<ApiResponse<AvailableEmployeesResponse>> GetAvailableEmployeesAsync(
            int businessId, 
            DateTime selectedDate, 
            TimeSpan selectedTime, 
            int totalDurationMinutes)
        {
            try
            {
                var queryString = $"?businessId={businessId}" +
                                 $"&selectedDate={selectedDate:yyyy-MM-dd}" +
                                 $"&selectedTime={selectedTime}" +
                                 $"&totalDurationMinutes={totalDurationMinutes}";
                
                var response = await _httpClient.GetAsync($"api/employees/available{queryString}");
                return await HandleApiResponse<AvailableEmployeesResponse>(response);
            }
            catch (Exception ex)
            {
                return new ApiResponse<AvailableEmployeesResponse> { Success = false, Message = $"Hata: {ex.Message}" };
            }
        }

        public async Task<ApiResponse<EmployeeDto?>> GetEmployeeByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/employees/{id}");
                return await HandleApiResponse<EmployeeDto?>(response);
            }
            catch (Exception ex)
            {
                return new ApiResponse<EmployeeDto?> { Success = false, Message = $"Hata: {ex.Message}" };
            }
        }

        public async Task<ApiResponse<EmployeeDto>> CreateEmployeeAsync(CreateEmployeeDto createEmployeeDto)
        {
            try
            {
                var request = await CreateRequestWithAuth(HttpMethod.Post, "api/employees");
                request.Content = JsonContent.Create(createEmployeeDto);
                var response = await _httpClient.SendAsync(request);
                return await HandleApiResponse<EmployeeDto>(response);
            }
            catch (Exception ex)
            {
                return new ApiResponse<EmployeeDto> { Success = false, Message = $"Hata: {ex.Message}" };
            }
        }

        public async Task<ApiResponse<EmployeeDto?>> UpdateEmployeeAsync(int id, CreateEmployeeDto updateEmployeeDto)
        {
            try
            {
                var request = await CreateRequestWithAuth(HttpMethod.Put, $"api/employees/{id}");
                request.Content = JsonContent.Create(updateEmployeeDto);
                var response = await _httpClient.SendAsync(request);
                return await HandleApiResponse<EmployeeDto?>(response);
            }
            catch (Exception ex)
            {
                return new ApiResponse<EmployeeDto?> { Success = false, Message = $"Hata: {ex.Message}" };
            }
        }

        public async Task<ApiResponse<bool>> DeleteEmployeeAsync(int id)
        {
            try
            {
                var request = await CreateRequestWithAuth(HttpMethod.Delete, $"api/employees/{id}");
                var response = await _httpClient.SendAsync(request);
                return await HandleApiResponse<bool>(response);
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool> { Success = false, Message = $"Hata: {ex.Message}" };
            }
        }
    }
}
