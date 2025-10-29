using AppointmentManagementSystem.Domain.Entities;
using AppointmentManagementSystem.Domain.Interfaces;
using AppointmentManagementSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AppointmentManagementSystem.Infrastructure.Repositories
{
    public class BusinessPhotoRepository : GenericRepository<BusinessPhoto>, IBusinessPhotoRepository
    {
        public BusinessPhotoRepository(AppointmentDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<BusinessPhoto>> GetByEntityIdAsync(int businessId)
        {
            return await _dbSet
                .Where(p => p.BusinessId == businessId && !p.IsDeleted)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }
    }

    public class EmployeePhotoRepository : GenericRepository<EmployeePhoto>, IEmployeePhotoRepository
    {
        public EmployeePhotoRepository(AppointmentDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<EmployeePhoto>> GetByEntityIdAsync(int employeeId)
        {
            return await _dbSet
                .Where(p => p.EmployeeId == employeeId && !p.IsDeleted)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }
    }

    public class ServicePhotoRepository : GenericRepository<ServicePhoto>, IServicePhotoRepository
    {
        public ServicePhotoRepository(AppointmentDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<ServicePhoto>> GetByEntityIdAsync(int serviceId)
        {
            return await _dbSet
                .Where(p => p.ServiceId == serviceId && !p.IsDeleted)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }
    }

    public class AppointmentPhotoRepository : GenericRepository<AppointmentPhoto>, IAppointmentPhotoRepository
    {
        public AppointmentPhotoRepository(AppointmentDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<AppointmentPhoto>> GetByEntityIdAsync(int appointmentId)
        {
            return await _dbSet
                .Where(p => p.AppointmentId == appointmentId && !p.IsDeleted)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }
    }
}
