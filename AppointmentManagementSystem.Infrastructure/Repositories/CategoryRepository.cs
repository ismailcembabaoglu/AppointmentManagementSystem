using AppointmentManagementSystem.Domain.Entities;
using AppointmentManagementSystem.Domain.Interfaces;
using AppointmentManagementSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AppointmentManagementSystem.Infrastructure.Repositories
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(AppointmentDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Category>> GetAllWithBusinessCountAsync()
        {
            return await _dbSet
                .Include(c => c.Businesses.Where(b => b.IsActive && !b.IsDeleted)) // ISDELETED EKLENDİ
                .Where(c => !c.IsDeleted) // ISDELETED EKLENDİ
                .Select(c => new Category
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    Icon = c.Icon,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt,
                    IsDeleted = c.IsDeleted
                })
                .ToListAsync();
        }

        public async Task<Category?> GetByIdWithBusinessesAsync(int id)
        {
            return await _dbSet
                .Include(c => c.Businesses.Where(b => b.IsActive && !b.IsDeleted)) // ISDELETED EKLENDİ
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted); // ISDELETED EKLENDİ
        }
    }
}
