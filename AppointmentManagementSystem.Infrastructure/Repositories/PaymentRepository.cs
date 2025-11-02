using AppointmentManagementSystem.Domain.Entities;
using AppointmentManagementSystem.Domain.Interfaces;
using AppointmentManagementSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AppointmentManagementSystem.Infrastructure.Repositories
{
    public class PaymentRepository : GenericRepository<Payment>, IPaymentRepository
    {
        public PaymentRepository(AppointmentDbContext context) : base(context)
        {
        }

        public async Task<Payment?> GetByMerchantOidAsync(string merchantOid)
        {
            return await _context.Set<Payment>()
                .Include(p => p.Business)
                .FirstOrDefaultAsync(p => p.MerchantOid == merchantOid);
        }

        public async Task<IEnumerable<Payment>> GetByBusinessIdAsync(int businessId)
        {
            return await _context.Set<Payment>()
                .Where(p => p.BusinessId == businessId)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Payment>> GetFailedPaymentsForRetryAsync()
        {
            var now = DateTime.UtcNow;
            return await _context.Set<Payment>()
                .Include(p => p.Business)
                    .ThenInclude(b => b!.Subscription)
                .Where(p => p.Status == PaymentStatus.Failed &&
                           p.RetryCount < p.MaxRetries &&
                           p.NextRetryDate.HasValue &&
                           p.NextRetryDate.Value <= now)
                .ToListAsync();
        }

        public async Task<IEnumerable<Payment>> GetPendingPaymentsAsync()
        {
            return await _context.Set<Payment>()
                .Include(p => p.Business)
                .Where(p => p.Status == PaymentStatus.Pending)
                .ToListAsync();
        }
    }
}