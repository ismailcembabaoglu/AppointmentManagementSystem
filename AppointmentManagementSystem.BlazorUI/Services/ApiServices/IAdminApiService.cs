using AppointmentManagementSystem.BlazorUI.Models;

namespace AppointmentManagementSystem.BlazorUI.Services.ApiServices
{
    public interface IAdminApiService
    {
        // Dashboard
        Task<AdminDashboardStats?> GetDashboardStatsAsync();

        // Businesses
        Task<List<BusinessAdminModel>?> GetAllBusinessesAsync(string? searchTerm = null, bool? isActive = null, int? categoryId = null);
        Task<BusinessDetailAdminModel?> GetBusinessDetailAsync(int businessId);
        Task<bool> UpdateBusinessStatusAsync(int businessId, bool isActive);

        // Appointments
        Task<List<AppointmentAdminModel>?> GetBusinessAppointmentsAsync(int businessId, DateTime? startDate = null, DateTime? endDate = null, string? status = null);
        Task<bool> DeleteAppointmentAsync(int appointmentId);
        Task<bool> UpdateAppointmentStatusAsync(int appointmentId, string status);

        // Employees
        Task<bool> DeleteEmployeeAsync(int employeeId);

        // Payments
        Task<List<PaymentAdminModel>?> GetBusinessPaymentsAsync(int businessId, DateTime? startDate = null, DateTime? endDate = null);
        Task<bool> UpdateSubscriptionAutoRenewalAsync(int businessId, bool autoRenewal);
        Task<bool> RefundPaymentAsync(int paymentId, string reason);

        // Reports
        Task<ReportsDataModel?> GetReportsAsync(DateTime? startDate = null, DateTime? endDate = null);
    }
}
