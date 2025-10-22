using AppointmentManagementSystem.Domain.Entities;

namespace AppointmentManagementSystem.Domain.Interfaces
{
    public interface IServiceRepository : IRepository<Service>
    {
        Task<IEnumerable<Service>> GetByBusinessAsync(int businessId);
        Task<IEnumerable<Service>> GetAllWithBusinessAsync(int? businessId = null);
        Task<Service?> GetByIdWithBusinessAsync(int id);
    }
}
