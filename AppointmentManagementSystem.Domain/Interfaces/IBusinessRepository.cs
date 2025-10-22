using AppointmentManagementSystem.Domain.Entities;

namespace AppointmentManagementSystem.Domain.Interfaces
{
    public interface IBusinessRepository : IRepository<Business>
    {
        Task<IEnumerable<Business>> GetAllWithDetailsAsync(int? categoryId = null, string? searchTerm = null);
        Task<Business?> GetByIdWithDetailsAsync(int id);
        Task<IEnumerable<Business>> GetByCategoryAsync(int categoryId);
        Task<double?> GetAverageRatingAsync(int businessId);
    }
}
