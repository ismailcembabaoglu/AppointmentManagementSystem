using AppointmentManagementSystem.API.Hubs;
using AppointmentManagementSystem.Application.DTOs;
using Microsoft.AspNetCore.SignalR;

namespace AppointmentManagementSystem.API.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(IHubContext<NotificationHub> hubContext, ILogger<NotificationService> logger)
        {
            _hubContext = hubContext;
            _logger = logger;
        }

        public async Task NotifyAppointmentCreated(AppointmentDto appointment, int customerId, int businessId)
        {
            try
            {
                // İşletmeye bildirim gönder (pending appointment)
                await _hubContext.Clients.Group($"user_{businessId}")
                    .SendAsync("AppointmentCreated", appointment);

                // Müşteriye bildirim gönder
                await _hubContext.Clients.Group($"user_{customerId}")
                    .SendAsync("AppointmentCreated", appointment);

                _logger.LogInformation($"Appointment created notification sent. ID: {appointment.Id}, Customer: {customerId}, Business: {businessId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending appointment created notification");
            }
        }

        public async Task NotifyAppointmentStatusChanged(AppointmentDto appointment, int customerId, int businessId)
        {
            try
            {
                // Her iki tarafa da durum değişikliğini bildir
                await _hubContext.Clients.Group($"user_{businessId}")
                    .SendAsync("AppointmentStatusChanged", appointment);

                await _hubContext.Clients.Group($"user_{customerId}")
                    .SendAsync("AppointmentStatusChanged", appointment);

                _logger.LogInformation($"Appointment status changed notification sent. ID: {appointment.Id}, Status: {appointment.Status}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending appointment status changed notification");
            }
        }

        public async Task NotifyAppointmentDeleted(int appointmentId, int customerId, int businessId)
        {
            try
            {
                // Randevu silindi bildirimi
                await _hubContext.Clients.Group($"user_{businessId}")
                    .SendAsync("AppointmentDeleted", appointmentId);

                await _hubContext.Clients.Group($"user_{customerId}")
                    .SendAsync("AppointmentDeleted", appointmentId);

                _logger.LogInformation($"Appointment deleted notification sent. ID: {appointmentId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending appointment deleted notification");
            }
        }
    }
}
