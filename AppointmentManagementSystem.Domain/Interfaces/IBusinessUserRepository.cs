using AppointmentManagementSystem.Domain.Entities;

namespace AppointmentManagementSystem.Domain.Interfaces
{
    public interface IBusinessUserRepository : IRepository<BusinessUser>
    {
        Task<BusinessUser?> GetByIdWithDetailsAsync(int id);
    }
}
