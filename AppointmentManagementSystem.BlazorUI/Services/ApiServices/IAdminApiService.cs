using AppointmentManagementSystem.BlazorUI.Models;

namespace AppointmentManagementSystem.BlazorUI.Services.ApiServices
{
    public interface IAdminApiService
    {
        // Dashboard
        Task<ApiResponse<AdminDashboardStats?>> GetDashboardStatsAsync();

        // Businesses
        Task<ApiResponse<List<BusinessAdminModel>?>> GetAllBusinessesAsync(string? searchTerm = null, bool? isActive = null, int? categoryId = null);
        Task<ApiResponse<BusinessDetailAdminModel?>> GetBusinessDetailAsync(int businessId);
        Task<ApiResponse<bool>> UpdateBusinessStatusAsync(int businessId, bool isActive);

        // Appointments
        Task<ApiResponse<List<AppointmentAdminModel>?>> GetBusinessAppointmentsAsync(int businessId, DateTime? startDate = null, DateTime? endDate = null, string? status = null);
        Task<ApiResponse<bool>> DeleteAppointmentAsync(int appointmentId);
        Task<ApiResponse<bool>> UpdateAppointmentStatusAsync(int appointmentId, string status);

        // Employees
        Task<ApiResponse<bool>> DeleteEmployeeAsync(int employeeId);

        // Payments
        Task<ApiResponse<List<PaymentAdminModel>?>> GetBusinessPaymentsAsync(int businessId, DateTime? startDate = null, DateTime? endDate = null);
        Task<ApiResponse<bool>> UpdateSubscriptionAutoRenewalAsync(int businessId, bool autoRenewal);
        Task<ApiResponse<bool>> RefundPaymentAsync(int paymentId, string reason);

        // Reports
        Task<ApiResponse<ReportsDataModel?>> GetReportsAsync(DateTime? startDate = null, DateTime? endDate = null);
    }
}
