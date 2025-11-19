using AppointmentManagementSystem.Application.DTOs;
using AppointmentManagementSystem.BlazorUI.Models;
using AppointmentManagementSystem.Domain.Entities;
using Blazored.LocalStorage;
using System.Net.Http.Json;

namespace AppointmentManagementSystem.BlazorUI.Services.ApiServices
{
    public class AdminApiService : BaseApiService,IAdminApiService
    {

        public AdminApiService(HttpClient httpClient, ILocalStorageService localStorage) 
            : base(httpClient, localStorage)
        {
        }

        public async Task<ApiResponse<AdminDashboardStats?>> GetDashboardStatsAsync()
        {

            try
            {
                var request = await CreateRequestWithAuth(HttpMethod.Get, $"api/Admin/dashboard/stats");
                var response = await _httpClient.SendAsync(request);
                return await HandleApiResponse<AdminDashboardStats?>(response);
            }
            catch (Exception ex)
            {
                return new ApiResponse<AdminDashboardStats?> { Success = false, Message = $"Hata: {ex.Message}" };
            }

            //try
            //{

            //    return await _httpClient.GetFromJsonAsync<AdminDashboardStats>("api/Admin/dashboard/stats");
            //}
            //catch
            //{
            //    return null;
            //}
        }

        public async Task<ApiResponse< List<BusinessAdminModel>?>> GetAllBusinessesAsync(string? searchTerm = null, bool? isActive = null, int? categoryId = null)
        {

            try
            {
                var queryParams = new List<string>();
                if (!string.IsNullOrEmpty(searchTerm))
                    queryParams.Add($"searchTerm={Uri.EscapeDataString(searchTerm)}");
                if (isActive.HasValue)
                    queryParams.Add($"isActive={isActive.Value}");
                if (categoryId.HasValue)
                    queryParams.Add($"categoryId={categoryId.Value}");

                var query = queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : "";
                var request = await CreateRequestWithAuth(HttpMethod.Get, $"api/Admin/businesses{query}");
                var response = await _httpClient.SendAsync(request);
                return await HandleApiResponse<List<BusinessAdminModel>?>(response);
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<BusinessAdminModel>?> { Success = false, Message = $"Hata: {ex.Message}" };
            }
        }

        public async Task<ApiResponse<BusinessDetailAdminModel?>> GetBusinessDetailAsync(int businessId)
        {
            try
            {
                var request = await CreateRequestWithAuth(HttpMethod.Get, $"api/Admin/businesses/{businessId}");
                var response = await _httpClient.SendAsync(request);
                return await HandleApiResponse<BusinessDetailAdminModel?>(response);
            }
            catch (Exception ex)
            {
                return new ApiResponse<BusinessDetailAdminModel?> { Success = false, Message = $"Hata: {ex.Message}" };
            }
        }

        public async Task<ApiResponse<bool>> UpdateBusinessStatusAsync(int businessId, bool isActive)
        {
            try
            {
                var request = await CreateRequestWithAuth(HttpMethod.Put, $"api/Admin/businesses/{businessId}/status");
                request.Content = JsonContent.Create( new { IsActive = isActive });
                var response = await _httpClient.SendAsync(request);
                return await HandleApiResponse<bool>(response);
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool> { Success = false, Message = $"Hata: {ex.Message}" };
            }
            //try
            //{
            //    var response = await _httpClient.PutAsJsonAsync($"api/Admin/businesses/{businessId}/status", new { IsActive = isActive });
            //    return response.IsSuccessStatusCode;
            //}
            //catch
            //{
            //    return false;
            //}
        }

        public async Task<ApiResponse<List<AppointmentAdminModel>?>> GetBusinessAppointmentsAsync(int businessId, DateTime? startDate = null, DateTime? endDate = null, string? status = null)
        {
            try
            {
                var queryParams = new List<string>();
                if (startDate.HasValue)
                    queryParams.Add($"startDate={startDate.Value:yyyy-MM-dd}");
                if (endDate.HasValue)
                    queryParams.Add($"endDate={endDate.Value:yyyy-MM-dd}");
                if (!string.IsNullOrEmpty(status))
                    queryParams.Add($"status={Uri.EscapeDataString(status)}");

                var query = queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : "";
                var request = await CreateRequestWithAuth(HttpMethod.Get, $"api/Admin/businesses/{businessId}/appointments{query}");
                var response = await _httpClient.SendAsync(request);
                return await HandleApiResponse<List<AppointmentAdminModel>?>(response);
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<AppointmentAdminModel>?> { Success = false, Message = $"Hata: {ex.Message}" };
            }
        }

        public async Task<ApiResponse<bool>> DeleteAppointmentAsync(int appointmentId)
        {
            try
            {
                var request = await CreateRequestWithAuth(HttpMethod.Delete, $"api/Admin/appointments/{appointmentId}");
                var response = await _httpClient.SendAsync(request);
                return await HandleApiResponse<bool>(response);
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool> { Success = false, Message = $"Hata: {ex.Message}" };
            }
        }

        public async Task<ApiResponse<bool>> UpdateAppointmentStatusAsync(int appointmentId, string status)
        {
            try
            {
                var request = await CreateRequestWithAuth(HttpMethod.Put, $"api/Admin/appointments/{appointmentId}/status");
                request.Content = JsonContent.Create(new { Status = status });
                var response = await _httpClient.SendAsync(request);
                return await HandleApiResponse<bool>(response);
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool> { Success = false, Message = $"Hata: {ex.Message}" };
            }
           
        }

        public async Task<ApiResponse<bool>> DeleteEmployeeAsync(int employeeId)
        {
            try
            {
                var request = await CreateRequestWithAuth(HttpMethod.Delete, $"api/Admin/employees/{employeeId}");
                var response = await _httpClient.SendAsync(request);
                return await HandleApiResponse<bool>(response);
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool> { Success = false, Message = $"Hata: {ex.Message}" };
            }
         
        }

        public async Task<ApiResponse<List<PaymentAdminModel>?>> GetBusinessPaymentsAsync(int businessId, DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                var queryParams = new List<string>();
                if (startDate.HasValue)
                    queryParams.Add($"startDate={startDate.Value:yyyy-MM-dd}");
                if (endDate.HasValue)
                    queryParams.Add($"endDate={endDate.Value:yyyy-MM-dd}");

                var query = queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : "";
                var request = await CreateRequestWithAuth(HttpMethod.Get, $"api/Admin/businesses/{businessId}/payments{query}");
                var response = await _httpClient.SendAsync(request);
                return await HandleApiResponse<List<PaymentAdminModel>?>(response);
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<PaymentAdminModel>?> { Success = false, Message = $"Hata: {ex.Message}" };
            }
            
        }

        public async Task<ApiResponse<bool>> UpdateSubscriptionAutoRenewalAsync(int businessId, bool autoRenewal)
        {
            try
            {
                var request = await CreateRequestWithAuth(HttpMethod.Put, $"api/Admin/businesses/{businessId}/subscription/auto-renewal");
                request.Content = JsonContent.Create(new { AutoRenewal = autoRenewal });
                var response = await _httpClient.SendAsync(request);
                return await HandleApiResponse<bool>(response);
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool> { Success = false, Message = $"Hata: {ex.Message}" };
            }
           
        }

        public async Task<ApiResponse<bool>> RefundPaymentAsync(int paymentId, string reason)
        {

            try
            {
                var request = await CreateRequestWithAuth(HttpMethod.Post, $"api/Admin/payments/{paymentId}/refund");
                request.Content = JsonContent.Create(new { Reason = reason });
                var response = await _httpClient.SendAsync(request);
                return await HandleApiResponse<bool>(response);
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool> { Success = false, Message = $"Hata: {ex.Message}" };
            }
           
        }

        public async Task<ApiResponse<ReportsDataModel?>> GetReportsAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                var queryParams = new List<string>();
                if (startDate.HasValue)
                    queryParams.Add($"startDate={startDate.Value:yyyy-MM-dd}");
                if (endDate.HasValue)
                    queryParams.Add($"endDate={endDate.Value:yyyy-MM-dd}");

                var query = queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : "";

                var request = await CreateRequestWithAuth(HttpMethod.Get, $"api/Admin/reports{query}");
                var response = await _httpClient.SendAsync(request);
                return await HandleApiResponse<ReportsDataModel?>(response);
                //return await _httpClient.GetFromJsonAsync<ReportsDataModel>($"api/Admin/reports{query}");
            }
            catch (Exception ex)
            {
                return new ApiResponse<ReportsDataModel?> { Success = false, Message = $"Hata: {ex.Message}" };
            }
        }
    }
}
