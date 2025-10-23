using AppointmentManagementSystem.Domain.Entities;
using AppointmentManagementSystem.Domain.Interfaces;
using AppointmentManagementSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AppointmentManagementSystem.Infrastructure.Repositories
{
    public class BusinessUserRepository : GenericRepository<BusinessUser>, IBusinessUserRepository
    {
        public BusinessUserRepository(AppointmentDbContext context) : base(context)
        {
        }

        public async Task<BusinessUser?> GetByIdWithDetailsAsync(int id)
        {
            return await _dbSet
                .Include(bu => bu.Business)
                .Include(bu => bu.User)
                .FirstOrDefaultAsync(bu => bu.Id == id && !bu.IsDeleted);
        }
    }
}
