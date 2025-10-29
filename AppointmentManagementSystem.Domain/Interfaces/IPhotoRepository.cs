using AppointmentManagementSystem.Domain.Entities;

namespace AppointmentManagementSystem.Domain.Interfaces
{
    public interface IPhotoRepository<T> : IRepository<T> where T : Photo
    {
        Task<IEnumerable<T>> GetByEntityIdAsync(int entityId);
    }

    public interface IBusinessPhotoRepository : IPhotoRepository<BusinessPhoto> { }
    public interface IEmployeePhotoRepository : IPhotoRepository<EmployeePhoto> { }
    public interface IServicePhotoRepository : IPhotoRepository<ServicePhoto> { }
    public interface IAppointmentPhotoRepository : IPhotoRepository<AppointmentPhoto> { }
}
