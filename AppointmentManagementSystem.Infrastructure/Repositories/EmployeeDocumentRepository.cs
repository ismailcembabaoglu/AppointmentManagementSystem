using AppointmentManagementSystem.Domain.Entities;
using AppointmentManagementSystem.Domain.Interfaces;
using AppointmentManagementSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AppointmentManagementSystem.Infrastructure.Repositories
{
    public class EmployeeDocumentRepository : GenericRepository<EmployeeDocument>, IEmployeeDocumentRepository
    {
        public EmployeeDocumentRepository(AppointmentDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<EmployeeDocument>> GetByEmployeeIdAsync(int employeeId)
        {
            return await _dbSet
                .Where(d => d.EmployeeId == employeeId && !d.IsDeleted)
                .OrderByDescending(d => d.CreatedAt)
                .ToListAsync();
        }
    }
}
