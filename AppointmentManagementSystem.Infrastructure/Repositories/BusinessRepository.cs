using AppointmentManagementSystem.Domain.Entities;
using AppointmentManagementSystem.Domain.Interfaces;
using AppointmentManagementSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AppointmentManagementSystem.Infrastructure.Repositories
{
    public class BusinessRepository : GenericRepository<Business>, IBusinessRepository
    {
        public BusinessRepository(AppointmentDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Business>> GetAllWithDetailsAsync(int? categoryId = null, string? searchTerm = null, string? city = null, string? district = null)
        {
            var query = _dbSet
                .Include(b => b.Category)
                .Include(b => b.Photos)
                .AsQueryable();

            if (categoryId.HasValue)
                query = query.Where(b => b.CategoryId == categoryId.Value);

            if (!string.IsNullOrEmpty(searchTerm))
                query = query.Where(b => b.Name.Contains(searchTerm) ||
                                       (b.Description != null && b.Description.Contains(searchTerm)) ||
                                       (b.City != null && b.City.Contains(searchTerm)));

            if (!string.IsNullOrEmpty(city))
                query = query.Where(b => b.City == city);

            if (!string.IsNullOrEmpty(district))
                query = query.Where(b => b.District == district);

            return await query
                .Where(b => b.IsActive && !b.IsDeleted)
                .ToListAsync();
        }

        public async Task<Business?> GetByIdWithDetailsAsync(int id)
        {
            return await _dbSet
           .Include(b => b.Category)
           .Include(b => b.Photos)
           .FirstOrDefaultAsync(b => b.Id == id && b.IsActive && !b.IsDeleted);
        }

        public async Task<IEnumerable<Business>> GetByCategoryAsync(int categoryId)
        {
            return await _dbSet
                .Include(b => b.Category)
                .Include(b => b.Photos)
                .Where(b => b.CategoryId == categoryId && b.IsActive && !b.IsDeleted)
                .ToListAsync();
        }

        public async Task<double?> GetAverageRatingAsync(int businessId)
        {
            var appointments = await _context.Set<Appointment>()
                .Where(a => a.BusinessId == businessId && a.Rating.HasValue && !a.IsDeleted)
                .ToListAsync();

            if (!appointments.Any())
                return null;

            return appointments.Average(a => a.Rating);
        }
        public async Task<bool> CategoryExistsAsync(int categoryId)
        {
            return await _context.Set<Category>().AnyAsync(c => c.Id == categoryId && !c.IsDeleted);
        }
    }
}
