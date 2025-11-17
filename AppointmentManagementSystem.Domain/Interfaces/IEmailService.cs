namespace AppointmentManagementSystem.Domain.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailVerificationAsync(string toEmail, string toName, string verificationToken);
        Task SendWelcomeEmailAsync(string toEmail, string toName);
        Task SendAppointmentConfirmationAsync(
            string toEmail, 
            string customerName, 
            string businessName, 
            string serviceName, 
            DateTime appointmentDate, 
            TimeSpan startTime, 
            TimeSpan endTime,
            string? employeeName,
            string? notes);
    }
}
