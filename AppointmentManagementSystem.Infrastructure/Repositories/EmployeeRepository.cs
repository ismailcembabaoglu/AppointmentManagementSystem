using AppointmentManagementSystem.Domain.Entities;
using AppointmentManagementSystem.Domain.Interfaces;
using AppointmentManagementSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AppointmentManagementSystem.Infrastructure.Repositories
{
    public class EmployeeRepository : GenericRepository<Employee>, IEmployeeRepository
    {
        public EmployeeRepository(AppointmentDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Employee>> GetByBusinessAsync(int businessId)
        {
            return await _dbSet
                .Include(e => e.Business)
                .Include(e => e.Photos)
                .Include(e => e.Documents)
                .Where(e => e.BusinessId == businessId && e.IsActive && !e.IsDeleted) // ISDELETED EKLENDİ
                .ToListAsync();
        }

        public async Task<IEnumerable<Employee>> GetAllWithBusinessAsync(int? businessId = null)
        {
            var query = _dbSet
                .Include(e => e.Business)
                .Include(e => e.Photos)
                .Include(e => e.Documents)
                .AsQueryable();

            if (businessId.HasValue)
                query = query.Where(e => e.BusinessId == businessId.Value);

            return await query
                .Where(e => e.IsActive && !e.IsDeleted) // ISDELETED EKLENDİ
                .ToListAsync();
        }

        public async Task<Employee?> GetByIdWithBusinessAsync(int id)
        {
            return await _dbSet
                .Include(e => e.Business)
                .Include(e => e.Photos)
                .Include(e => e.Documents)
                .FirstOrDefaultAsync(e => e.Id == id && e.IsActive && !e.IsDeleted); // ISDELETED EKLENDİ
        }
    }
}
