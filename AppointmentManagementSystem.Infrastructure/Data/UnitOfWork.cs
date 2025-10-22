using AppointmentManagementSystem.Domain.Interfaces;
using System.Threading.Tasks;

namespace AppointmentManagementSystem.Infrastructure.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppointmentDbContext _context;

        public UnitOfWork(AppointmentDbContext context) // BU SATIRI GÜNCELLE
        {
            _context = context;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
