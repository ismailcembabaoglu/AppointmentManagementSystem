namespace AppointmentManagementSystem.Domain.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailVerificationAsync(string toEmail, string toName, string verificationToken);
        Task SendWelcomeEmailAsync(string toEmail, string toName);
    }
}
