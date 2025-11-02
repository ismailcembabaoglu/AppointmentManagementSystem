using AppointmentManagementSystem.Domain.Entities;
using AppointmentManagementSystem.Domain.Interfaces;
using AppointmentManagementSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AppointmentManagementSystem.Infrastructure.Repositories
{
    public class BusinessSubscriptionRepository : GenericRepository<BusinessSubscription>, IBusinessSubscriptionRepository
    {
        public BusinessSubscriptionRepository(AppointmentDbContext context) : base(context)
        {
        }

        public async Task<BusinessSubscription?> GetByBusinessIdAsync(int businessId)
        {
            return await _context.Set<BusinessSubscription>()
                .Include(s => s.Business)
                .FirstOrDefaultAsync(s => s.BusinessId == businessId);
        }

        public async Task<IEnumerable<BusinessSubscription>> GetSubscriptionsDueForBillingAsync(DateTime date)
        {
            return await _context.Set<BusinessSubscription>()
                .Include(s => s.Business)
                .Where(s => s.IsActive &&
                           s.SubscriptionStatus == SubscriptionStatus.Active &&
                           s.NextBillingDate.HasValue &&
                           s.NextBillingDate.Value.Date <= date.Date)
                .ToListAsync();
        }

        public async Task<IEnumerable<BusinessSubscription>> GetActiveSubscriptionsAsync()
        {
            return await _context.Set<BusinessSubscription>()
                .Include(s => s.Business)
                .Where(s => s.IsActive && s.SubscriptionStatus == SubscriptionStatus.Active)
                .ToListAsync();
        }
    }
}