using AppointmentManagementSystem.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace AppointmentManagementSystem.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly string _smtpHost;
        private readonly int _smtpPort;
        private readonly string _smtpUsername;
        private readonly string _smtpPassword;
        private readonly string _fromEmail;
        private readonly string _fromName;
        private readonly string _appUrl;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
            _smtpHost = _configuration["EmailSettings:SmtpHost"] ?? "";
            _smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"] ?? "587");
            _smtpUsername = _configuration["EmailSettings:SmtpUsername"] ?? "";
            _smtpPassword = _configuration["EmailSettings:SmtpPassword"] ?? "";
            _fromEmail = _configuration["EmailSettings:FromEmail"] ?? "";
            _fromName = _configuration["EmailSettings:FromName"] ?? "";
            _appUrl = _configuration["EmailSettings:AppUrl"] ?? "https://aptivaplan.com.tr";
        }

        public async Task SendEmailVerificationAsync(string toEmail, string toName, string verificationToken)
        {
            var verificationUrl = $"{_appUrl}/verify-email?token={verificationToken}";
            var subject = "Email Adresinizi Doğrulayın - Aptiva Plan";
            var body = GetVerificationEmailTemplate(toName, verificationUrl);

            await SendEmailAsync(toEmail, subject, body);
        }

        public async Task SendWelcomeEmailAsync(string toEmail, string toName)
        {
            var subject = "Hoş Geldiniz - Aptiva Plan";
            var body = GetWelcomeEmailTemplate(toName);

            await SendEmailAsync(toEmail, subject, body);
        }

        private async Task SendEmailAsync(string toEmail, string subject, string htmlBody)
        {
            try
            {
                using var mailMessage = new MailMessage
                {
                    From = new MailAddress(_fromEmail, _fromName),
                    Subject = subject,
                    Body = htmlBody,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(toEmail);

                using var client = new SmtpClient(_smtpHost, _smtpPort)
                {
                    EnableSsl = true,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(_smtpUsername, _smtpPassword),
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Timeout = 30000 // 30 saniye timeout
                };

                // Port 465 için özel ayar (Implicit SSL)
                if (_smtpPort == 465)
                {
                    // .NET'te port 465 için MailKit kullanmak daha iyi olur
                    // Ama standart SmtpClient ile deneme
                    client.EnableSsl = true;
                }

                await client.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                // Detaylı hata logu
                Console.WriteLine($"Email gönderim hatası: {ex.Message}");
                Console.WriteLine($"Inner Exception: {ex.InnerException?.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                throw new Exception($"Email gönderilemedi: {ex.Message}", ex);
            }
        }

        private string GetVerificationEmailTemplate(string userName, string verificationUrl)
        {
            return $@"
<!DOCTYPE html>
<html lang=""tr"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Email Doğrulama</title>
</head>
<body style=""margin: 0; padding: 0; font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; background-color: #f4f7fa;"">
    <table width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""background-color: #f4f7fa; padding: 40px 0;"">
        <tr>
            <td align=""center"">
                <table width=""600"" cellpadding=""0"" cellspacing=""0"" style=""background-color: #ffffff; border-radius: 12px; box-shadow: 0 4px 6px rgba(0,0,0,0.1); overflow: hidden;"">
                    <!-- Header -->
                    <tr>
                        <td style=""background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); padding: 40px 30px; text-align: center;"">
                            <h1 style=""color: #ffffff; margin: 0; font-size: 28px; font-weight: 600;"">
                                Aptiva Plan
                            </h1>
                            <p style=""color: #ffffff; margin: 10px 0 0 0; font-size: 16px; opacity: 0.9;"">
                                Randevu Yönetim Sistemi
                            </p>
                        </td>
                    </tr>
                    
                    <!-- Content -->
                    <tr>
                        <td style=""padding: 50px 40px;"">
                            <h2 style=""color: #2d3748; margin: 0 0 20px 0; font-size: 24px; font-weight: 600;"">
                                Merhaba {userName},
                            </h2>
                            
                            <p style=""color: #4a5568; line-height: 1.6; font-size: 16px; margin: 0 0 25px 0;"">
                                Aptiva Plan'a hoş geldiniz! Hesabınızı oluşturduğunuz için teşekkür ederiz.
                            </p>
                            
                            <p style=""color: #4a5568; line-height: 1.6; font-size: 16px; margin: 0 0 30px 0;"">
                                Hesabınızı aktif hale getirmek ve tüm özelliklerimizden yararlanmaya başlamak için 
                                lütfen aşağıdaki butona tıklayarak email adresinizi doğrulayın.
                            </p>
                            
                            <!-- Button -->
                            <table width=""100%"" cellpadding=""0"" cellspacing=""0"">
                                <tr>
                                    <td align=""center"" style=""padding: 20px 0;"">
                                        <a href=""{verificationUrl}"" 
                                           style=""display: inline-block; 
                                                  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); 
                                                  color: #ffffff; 
                                                  text-decoration: none; 
                                                  padding: 16px 48px; 
                                                  border-radius: 8px; 
                                                  font-size: 16px; 
                                                  font-weight: 600;
                                                  box-shadow: 0 4px 12px rgba(102, 126, 234, 0.4);
                                                  transition: all 0.3s ease;"">
                                            Email Adresimi Doğrula
                                        </a>
                                    </td>
                                </tr>
                            </table>
                            
                            <div style=""background-color: #f7fafc; border-left: 4px solid #667eea; padding: 20px; margin: 30px 0; border-radius: 4px;"">
                                <p style=""color: #2d3748; font-size: 14px; margin: 0 0 10px 0; font-weight: 600;"">
                                    ⏰ Önemli Bilgi:
                                </p>
                                <p style=""color: #4a5568; font-size: 14px; margin: 0; line-height: 1.5;"">
                                    Bu doğrulama linki <strong>24 saat</strong> boyunca geçerlidir. 
                                    Süre sonunda yeni bir doğrulama linki talep etmeniz gerekecektir.
                                </p>
                            </div>
                            
                            <p style=""color: #718096; font-size: 14px; line-height: 1.6; margin: 20px 0 0 0;"">
                                Eğer bu kaydı siz oluşturmadıysanız, bu emaili görmezden gelebilirsiniz.
                            </p>
                        </td>
                    </tr>
                    
                    <!-- Footer -->
                    <tr>
                        <td style=""background-color: #f7fafc; padding: 30px 40px; border-top: 1px solid #e2e8f0;"">
                            <p style=""color: #718096; font-size: 13px; line-height: 1.5; margin: 0 0 10px 0; text-align: center;"">
                                Bu otomatik bir emaildir, lütfen yanıtlamayın.
                            </p>
                            <p style=""color: #a0aec0; font-size: 12px; margin: 0; text-align: center;"">
                                © 2025 Aptiva Plan. Tüm hakları saklıdır.
                            </p>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>
</html>";
        }

        private string GetWelcomeEmailTemplate(string userName)
        {
            return $@"
<!DOCTYPE html>
<html lang=""tr"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Hoş Geldiniz</title>
</head>
<body style=""margin: 0; padding: 0; font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; background-color: #f4f7fa;"">
    <table width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""background-color: #f4f7fa; padding: 40px 0;"">
        <tr>
            <td align=""center"">
                <table width=""600"" cellpadding=""0"" cellspacing=""0"" style=""background-color: #ffffff; border-radius: 12px; box-shadow: 0 4px 6px rgba(0,0,0,0.1); overflow: hidden;"">
                    <tr>
                        <td style=""background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); padding: 40px 30px; text-align: center;"">
                            <h1 style=""color: #ffffff; margin: 0; font-size: 28px; font-weight: 600;"">
                                🎉 Hoş Geldiniz!
                            </h1>
                        </td>
                    </tr>
                    
                    <tr>
                        <td style=""padding: 50px 40px; text-align: center;"">
                            <h2 style=""color: #2d3748; margin: 0 0 20px 0; font-size: 24px;"">
                                Merhaba {userName}!
                            </h2>
                            
                            <p style=""color: #4a5568; line-height: 1.6; font-size: 16px; margin: 0 0 25px 0;"">
                                Email adresiniz başarıyla doğrulandı. Artık Aptiva Plan'ın tüm özelliklerinden 
                                yararlanabilirsiniz!
                            </p>
                            
                            <div style=""background-color: #f0fff4; border: 2px solid #9ae6b4; padding: 25px; margin: 30px 0; border-radius: 8px;"">
                                <p style=""color: #2f855a; font-size: 18px; margin: 0; font-weight: 600;"">
                                    ✓ Hesabınız Aktif!
                                </p>
                            </div>
                        </td>
                    </tr>
                    
                    <tr>
                        <td style=""background-color: #f7fafc; padding: 30px 40px;"">
                            <p style=""color: #a0aec0; font-size: 12px; margin: 0; text-align: center;"">
                                © 2025 Aptiva Plan. Tüm hakları saklıdır.
                            </p>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>
</html>";
        }
    }
}
