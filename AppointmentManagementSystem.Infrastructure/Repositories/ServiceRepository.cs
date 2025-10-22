using AppointmentManagementSystem.Domain.Entities;
using AppointmentManagementSystem.Domain.Interfaces;
using AppointmentManagementSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AppointmentManagementSystem.Infrastructure.Repositories
{
    public class ServiceRepository : GenericRepository<Service>, IServiceRepository
    {
        public ServiceRepository(AppointmentDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Service>> GetByBusinessAsync(int businessId)
        {
            return await _dbSet
                .Include(s => s.Business)
                .Include(s => s.Photos)
                .Where(s => s.BusinessId == businessId && s.IsActive && !s.IsDeleted) // ISDELETED EKLENDİ
                .ToListAsync();
        }

        public async Task<IEnumerable<Service>> GetAllWithBusinessAsync(int? businessId = null)
        {
            var query = _dbSet
                .Include(s => s.Business)
                .Include(s => s.Photos)
                .AsQueryable();

            if (businessId.HasValue)
                query = query.Where(s => s.BusinessId == businessId.Value);

            return await query
                .Where(s => s.IsActive && !s.IsDeleted) // ISDELETED EKLENDİ
                .ToListAsync();
        }

        public async Task<Service?> GetByIdWithBusinessAsync(int id)
        {
            return await _dbSet
                .Include(s => s.Business)
                .Include(s => s.Photos)
                .FirstOrDefaultAsync(s => s.Id == id && s.IsActive && !s.IsDeleted); // ISDELETED EKLENDİ
        }
    }
}
