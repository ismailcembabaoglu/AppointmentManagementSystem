using AppointmentManagementSystem.Domain.Entities;
using AppointmentManagementSystem.Domain.Interfaces;
using AppointmentManagementSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AppointmentManagementSystem.Infrastructure.Repositories
{
    public class AppointmentRepository : GenericRepository<Appointment>, IAppointmentRepository
    {
        public AppointmentRepository(AppointmentDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Appointment>> GetByCustomerAsync(int customerId)
        {
            return await _dbSet
                .Include(a => a.Customer)
                .Include(a => a.Business)
                .Include(a => a.Employee)
                .Include(a => a.Service)
                .Include(a => a.Photos)
                .Where(a => a.CustomerId == customerId && !a.IsDeleted) // ISDELETED EKLENDİ
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetByBusinessAsync(int businessId)
        {
            return await _dbSet
                .Include(a => a.Customer)
                .Include(a => a.Business)
                .Include(a => a.Employee)
                .Include(a => a.Service)
                .Include(a => a.Photos)
                .Where(a => a.BusinessId == businessId && !a.IsDeleted) // ISDELETED EKLENDİ
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetAllWithDetailsAsync(int? customerId = null, int? businessId = null)
        {
            var query = _dbSet
                .Include(a => a.Customer)
                .Include(a => a.Business)
                .Include(a => a.Employee)
                .Include(a => a.Service)
                // Photos kaldırıldı - çok büyük veri çekiyor ve gereksiz
                .AsQueryable();

            if (customerId.HasValue)
                query = query.Where(a => a.CustomerId == customerId.Value);

            if (businessId.HasValue)
                query = query.Where(a => a.BusinessId == businessId.Value);

            return await query
                .Where(a => !a.IsDeleted) // ISDELETED EKLENDİ
                .ToListAsync();
        }

        public async Task<Appointment?> GetByIdWithDetailsAsync(int id)
        {
            return await _dbSet
                .Include(a => a.Customer)
                .Include(a => a.Business)
                .Include(a => a.Employee)
                .Include(a => a.Service)
                .Include(a => a.Photos)
                .FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted); // ISDELETED EKLENDİ
        }

        public async Task UpdateStatusAsync(int id, string status)
        {
            var appointment = await _dbSet.FindAsync(id);
            if (appointment != null && !appointment.IsDeleted) // ISDELETED KONTROLÜ EKLENDİ
            {
                appointment.Status = status;
                appointment.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task AddRatingAsync(int id, int rating, string? review)
        {
            var appointment = await _dbSet.FindAsync(id);
            if (appointment != null && !appointment.IsDeleted) // ISDELETED KONTROLÜ EKLENDİ
            {
                appointment.Rating = rating;
                appointment.Review = review;
                appointment.RatingDate = DateTime.UtcNow;
                appointment.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }
    }
}
