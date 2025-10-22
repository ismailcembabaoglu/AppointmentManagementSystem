using AppointmentManagementSystem.Domain.Entities;

namespace AppointmentManagementSystem.Domain.Interfaces
{
    public interface IEmployeeRepository : IRepository<Employee>
    {
        Task<IEnumerable<Employee>> GetByBusinessAsync(int businessId);
        Task<IEnumerable<Employee>> GetAllWithBusinessAsync(int? businessId = null);
        Task<Employee?> GetByIdWithBusinessAsync(int id);
    }
}
