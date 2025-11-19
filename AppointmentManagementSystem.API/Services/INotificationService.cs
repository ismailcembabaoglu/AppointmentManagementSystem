using AppointmentManagementSystem.Application.DTOs;

namespace AppointmentManagementSystem.API.Services
{
    public interface INotificationService
    {
        Task NotifyAppointmentCreated(AppointmentDto appointment, int customerId, int businessId);
        Task NotifyAppointmentStatusChanged(AppointmentDto appointment, int customerId, int businessId);
        Task NotifyAppointmentDeleted(int appointmentId, int customerId, int businessId);
    }
}
