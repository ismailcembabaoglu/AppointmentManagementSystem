using System.Threading.Tasks;

namespace AppointmentManagementSystem.Domain.Interfaces
{
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync();
    }
}
