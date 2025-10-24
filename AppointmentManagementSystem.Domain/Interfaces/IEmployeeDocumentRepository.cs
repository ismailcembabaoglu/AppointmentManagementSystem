using AppointmentManagementSystem.Domain.Entities;

namespace AppointmentManagementSystem.Domain.Interfaces
{
    public interface IEmployeeDocumentRepository : IRepository<EmployeeDocument>
    {
        Task<IEnumerable<EmployeeDocument>> GetByEmployeeIdAsync(int employeeId);
    }
}
