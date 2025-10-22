using AppointmentManagementSystem.Domain.Entities;

namespace AppointmentManagementSystem.Domain.Interfaces
{
    public interface ICategoryRepository : IRepository<Category>
    {
        Task<IEnumerable<Category>> GetAllWithBusinessCountAsync();
        Task<Category?> GetByIdWithBusinessesAsync(int id);
    }
}
