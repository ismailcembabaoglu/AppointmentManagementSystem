using AppointmentManagementSystem.BlazorUI.Models;
using System.Net.Http.Json;

namespace AppointmentManagementSystem.BlazorUI.Services.ApiServices
{
    public class AdminApiService : IAdminApiService
    {
        private readonly HttpClient _httpClient;

        public AdminApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<AdminDashboardStats?> GetDashboardStatsAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<AdminDashboardStats>("api/Admin/dashboard/stats");
            }
            catch
            {
                return null;
            }
        }

        public async Task<List<BusinessAdminModel>?> GetAllBusinessesAsync(string? searchTerm = null, bool? isActive = null, int? categoryId = null)
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
                return await _httpClient.GetFromJsonAsync<List<BusinessAdminModel>>($"api/Admin/businesses{query}");
            }
            catch
            {
                return null;
            }
        }

        public async Task<BusinessDetailAdminModel?> GetBusinessDetailAsync(int businessId)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<BusinessDetailAdminModel>($"api/Admin/businesses/{businessId}");
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> UpdateBusinessStatusAsync(int businessId, bool isActive)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"api/Admin/businesses/{businessId}/status", new { IsActive = isActive });
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<AppointmentAdminModel>?> GetBusinessAppointmentsAsync(int businessId, DateTime? startDate = null, DateTime? endDate = null, string? status = null)
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
                return await _httpClient.GetFromJsonAsync<List<AppointmentAdminModel>>($"api/Admin/businesses/{businessId}/appointments{query}");
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> DeleteAppointmentAsync(int appointmentId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/Admin/appointments/{appointmentId}");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateAppointmentStatusAsync(int appointmentId, string status)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"api/Admin/appointments/{appointmentId}/status", new { Status = status });
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteEmployeeAsync(int employeeId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/Admin/employees/{employeeId}");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<PaymentAdminModel>?> GetBusinessPaymentsAsync(int businessId, DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                var queryParams = new List<string>();
                if (startDate.HasValue)
                    queryParams.Add($"startDate={startDate.Value:yyyy-MM-dd}");
                if (endDate.HasValue)
                    queryParams.Add($"endDate={endDate.Value:yyyy-MM-dd}");

                var query = queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : "";
                return await _httpClient.GetFromJsonAsync<List<PaymentAdminModel>>($"api/Admin/businesses/{businessId}/payments{query}");
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> UpdateSubscriptionAutoRenewalAsync(int businessId, bool autoRenewal)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"api/Admin/businesses/{businessId}/subscription/auto-renewal", new { AutoRenewal = autoRenewal });
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> RefundPaymentAsync(int paymentId, string reason)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"api/Admin/payments/{paymentId}/refund", new { Reason = reason });
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<ReportsDataModel?> GetReportsAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                var queryParams = new List<string>();
                if (startDate.HasValue)
                    queryParams.Add($"startDate={startDate.Value:yyyy-MM-dd}");
                if (endDate.HasValue)
                    queryParams.Add($"endDate={endDate.Value:yyyy-MM-dd}");

                var query = queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : "";
                return await _httpClient.GetFromJsonAsync<ReportsDataModel>($"api/Admin/reports{query}");
            }
            catch
            {
                return null;
            }
        }
    }
}
