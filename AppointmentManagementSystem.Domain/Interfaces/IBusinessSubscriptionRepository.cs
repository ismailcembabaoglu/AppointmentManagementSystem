using AppointmentManagementSystem.Domain.Entities;

namespace AppointmentManagementSystem.Domain.Interfaces
{
    public interface IBusinessSubscriptionRepository : IRepository<BusinessSubscription>
    {
        Task<BusinessSubscription?> GetByBusinessIdAsync(int businessId);
        Task<IEnumerable<BusinessSubscription>> GetSubscriptionsDueForBillingAsync(DateTime date);
        Task<IEnumerable<BusinessSubscription>> GetActiveSubscriptionsAsync();
    }
}