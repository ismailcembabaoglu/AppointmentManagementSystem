using AppointmentManagementSystem.Domain.Entities;

namespace AppointmentManagementSystem.Domain.Interfaces
{
    public interface IPaymentRepository : IRepository<Payment>
    {
        Task<Payment?> GetByMerchantOidAsync(string merchantOid);
        Task<IEnumerable<Payment>> GetByBusinessIdAsync(int businessId);
        Task<IEnumerable<Payment>> GetFailedPaymentsForRetryAsync();
        Task<IEnumerable<Payment>> GetPendingPaymentsAsync();
    }
}