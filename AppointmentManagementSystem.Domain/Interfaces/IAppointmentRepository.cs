using AppointmentManagementSystem.Domain.Entities;

namespace AppointmentManagementSystem.Domain.Interfaces
{
    public interface IAppointmentRepository : IRepository<Appointment>
    {
        Task<IEnumerable<Appointment>> GetByCustomerAsync(int customerId);
        Task<IEnumerable<Appointment>> GetByBusinessAsync(int businessId);
        Task<IEnumerable<Appointment>> GetAllWithDetailsAsync(int? customerId = null, int? businessId = null);
        Task<Appointment?> GetByIdWithDetailsAsync(int id);
        Task UpdateStatusAsync(int id, string status);
        Task AddRatingAsync(int id, int rating, string? review);
    }
}
