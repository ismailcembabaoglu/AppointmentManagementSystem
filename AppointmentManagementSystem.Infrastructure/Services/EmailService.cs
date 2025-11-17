using AppointmentManagementSystem.Domain.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;

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

        public async Task SendAppointmentConfirmationAsync(
            string toEmail, 
            string customerName, 
            string businessName, 
            string serviceName, 
            DateTime appointmentDate, 
            TimeSpan startTime, 
            TimeSpan endTime,
            string? employeeName,
            string? notes)
        {
            var subject = $"Randevu Onayı - {businessName}";
            var body = GetAppointmentConfirmationTemplate(
                customerName, 
                businessName, 
                serviceName, 
                appointmentDate, 
                startTime, 
                endTime, 
                employeeName, 
                notes);

            await SendEmailAsync(toEmail, subject, body);
        }

        private async Task SendEmailAsync(string toEmail, string subject, string htmlBody)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(_fromName, _fromEmail));
                message.To.Add(new MailboxAddress("", toEmail));
                message.Subject = subject;

                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = htmlBody
                };
                message.Body = bodyBuilder.ToMessageBody();

                using var client = new SmtpClient();

                // Port 465 için SSL, port 587 için STARTTLS
                client.ServerCertificateValidationCallback = (sender, certificate, chain, errors) => true;
                await client.ConnectAsync("aptivaplan.com.tr", 465, SecureSocketOptions.SslOnConnect);
                await client.AuthenticateAsync(_smtpUsername, _smtpPassword);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
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

        private string GetAppointmentConfirmationTemplate(
            string customerName, 
            string businessName, 
            string serviceName, 
            DateTime appointmentDate, 
            TimeSpan startTime, 
            TimeSpan endTime,
            string? employeeName,
            string? notes)
        {
            var appointmentDateFormatted = appointmentDate.ToString("dd MMMM yyyy dddd", new System.Globalization.CultureInfo("tr-TR"));
            var startTimeFormatted = startTime.ToString(@"hh\:mm");
            var endTimeFormatted = endTime.ToString(@"hh\:mm");
            var employeeInfo = !string.IsNullOrEmpty(employeeName) ? $"<strong>{employeeName}</strong> ile" : "";
            var notesSection = !string.IsNullOrEmpty(notes) ? $@"
            <div style=""background-color: #fff8e1; border-left: 4px solid #ffa726; padding: 20px; margin: 25px 0; border-radius: 4px;"">
                <p style=""color: #e65100; font-size: 14px; margin: 0 0 8px 0; font-weight: 600;"">
                    📝 Randevu Notunuz:
                </p>
                <p style=""color: #5d4037; font-size: 14px; margin: 0; line-height: 1.5;"">
                    {notes}
                </p>
            </div>" : "";

            return $@"
<!DOCTYPE html>
<html lang=""tr"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Randevu Onayı</title>
</head>
<body style=""margin: 0; padding: 0; font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; background-color: #f0f4f8;"">
    <table width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""background-color: #f0f4f8; padding: 40px 0;"">
        <tr>
            <td align=""center"">
                <table width=""650"" cellpadding=""0"" cellspacing=""0"" style=""background-color: #ffffff; border-radius: 16px; box-shadow: 0 8px 24px rgba(0,0,0,0.12); overflow: hidden;"">
                    
                    <!-- Header with gradient -->
                    <tr>
                        <td style=""background: linear-gradient(135deg, #43cea2 0%, #185a9d 100%); padding: 50px 40px; text-align: center; position: relative;"">
                            <div style=""background: rgba(255,255,255,0.15); width: 80px; height: 80px; border-radius: 50%; display: inline-flex; align-items: center; justify-content: center; margin-bottom: 20px;"">
                                <span style=""font-size: 40px;"">✓</span>
                            </div>
                            <h1 style=""color: #ffffff; margin: 0; font-size: 32px; font-weight: 700; text-shadow: 0 2px 4px rgba(0,0,0,0.1);"">
                                Randevunuz Oluşturuldu!
                            </h1>
                            <p style=""color: #ffffff; margin: 12px 0 0 0; font-size: 17px; opacity: 0.95;"">
                                Randevu detaylarınız aşağıdadır
                            </p>
                        </td>
                    </tr>
                    
                    <!-- Greeting -->
                    <tr>
                        <td style=""padding: 40px 40px 20px 40px;"">
                            <h2 style=""color: #1a237e; margin: 0 0 16px 0; font-size: 24px; font-weight: 600;"">
                                Merhaba {customerName},
                            </h2>
                            <p style=""color: #455a64; line-height: 1.7; font-size: 16px; margin: 0;"">
                                <strong>{businessName}</strong> için randevunuz başarıyla oluşturuldu. Randevu detaylarınızı aşağıda bulabilirsiniz.
                            </p>
                        </td>
                    </tr>
                    
                    <!-- Appointment Details Card -->
                    <tr>
                        <td style=""padding: 0 40px 30px 40px;"">
                            <table width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""background: linear-gradient(135deg, #f5f7fa 0%, #c3cfe2 100%); border-radius: 12px; overflow: hidden; border: 2px solid #e3f2fd;"">
                                <tr>
                                    <td style=""padding: 30px;"">
                                        
                                        <!-- Business Name -->
                                        <div style=""margin-bottom: 24px; padding-bottom: 20px; border-bottom: 2px solid rgba(255,255,255,0.5);"">
                                            <p style=""color: #607d8b; font-size: 13px; margin: 0 0 6px 0; text-transform: uppercase; letter-spacing: 1px; font-weight: 600;"">
                                                İŞLETME
                                            </p>
                                            <p style=""color: #1a237e; font-size: 22px; margin: 0; font-weight: 700;"">
                                                🏢 {businessName}
                                            </p>
                                        </div>
                                        
                                        <!-- Service -->
                                        <div style=""margin-bottom: 24px; padding-bottom: 20px; border-bottom: 2px solid rgba(255,255,255,0.5);"">
                                            <p style=""color: #607d8b; font-size: 13px; margin: 0 0 6px 0; text-transform: uppercase; letter-spacing: 1px; font-weight: 600;"">
                                                HİZMET
                                            </p>
                                            <p style=""color: #1a237e; font-size: 20px; margin: 0; font-weight: 600;"">
                                                ✨ {serviceName}
                                            </p>
                                        </div>
                                        
                                        <!-- Date & Time -->
                                        <table width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""margin-bottom: 24px;"">
                                            <tr>
                                                <td width=""50%"" style=""padding-right: 10px; vertical-align: top;"">
                                                    <div style=""background: rgba(255,255,255,0.7); padding: 20px; border-radius: 8px; text-align: center;"">
                                                        <p style=""color: #607d8b; font-size: 12px; margin: 0 0 8px 0; text-transform: uppercase; letter-spacing: 1px; font-weight: 600;"">
                                                            TARİH
                                                        </p>
                                                        <p style=""color: #1a237e; font-size: 18px; margin: 0; font-weight: 700;"">
                                                            📅 {appointmentDateFormatted}
                                                        </p>
                                                    </div>
                                                </td>
                                                <td width=""50%"" style=""padding-left: 10px; vertical-align: top;"">
                                                    <div style=""background: rgba(255,255,255,0.7); padding: 20px; border-radius: 8px; text-align: center;"">
                                                        <p style=""color: #607d8b; font-size: 12px; margin: 0 0 8px 0; text-transform: uppercase; letter-spacing: 1px; font-weight: 600;"">
                                                            SAAT
                                                        </p>
                                                        <p style=""color: #1a237e; font-size: 18px; margin: 0; font-weight: 700;"">
                                                            🕐 {startTimeFormatted} - {endTimeFormatted}
                                                        </p>
                                                    </div>
                                                </td>
                                            </tr>
                                        </table>
                                        
                                        {(!string.IsNullOrEmpty(employeeName) ? $@"
                                        <!-- Employee -->
                                        <div style=""background: rgba(255,255,255,0.7); padding: 20px; border-radius: 8px; text-align: center;"">
                                            <p style=""color: #607d8b; font-size: 12px; margin: 0 0 8px 0; text-transform: uppercase; letter-spacing: 1px; font-weight: 600;"">
                                                UZMAN
                                            </p>
                                            <p style=""color: #1a237e; font-size: 18px; margin: 0; font-weight: 700;"">
                                                👤 {employeeName}
                                            </p>
                                        </div>" : "")}
                                        
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    
                    {notesSection}
                    
                    <!-- Important Info Box -->
                    <tr>
                        <td style=""padding: 0 40px 35px 40px;"">
                            <div style=""background: linear-gradient(135deg, #e3f2fd 0%, #bbdefb 100%); border-left: 5px solid #2196f3; padding: 25px; border-radius: 8px;"">
                                <p style=""color: #1565c0; font-size: 15px; margin: 0 0 12px 0; font-weight: 700;"">
                                    ⚠️ Önemli Hatırlatmalar:
                                </p>
                                <ul style=""color: #1976d2; font-size: 14px; margin: 0; padding-left: 20px; line-height: 1.8;"">
                                    <li style=""margin-bottom: 8px;"">Randevu saatinden <strong>10 dakika önce</strong> hazır olmanızı rica ederiz.</li>
                                    <li style=""margin-bottom: 8px;"">Randevunuzu iptal etmeniz gerekirse lütfen en az <strong>24 saat öncesinden</strong> bildirin.</li>
                                    <li>Geç kalma durumunda lütfen işletmeyi arayarak bilgilendirin.</li>
                                </ul>
                            </div>
                        </td>
                    </tr>
                    
                    <!-- Manage Appointment Button -->
                    <tr>
                        <td align=""center"" style=""padding: 0 40px 40px 40px;"">
                            <table cellpadding=""0"" cellspacing=""0"">
                                <tr>
                                    <td align=""center"" style=""background: linear-gradient(135deg, #43cea2 0%, #185a9d 100%); border-radius: 50px; box-shadow: 0 6px 20px rgba(67, 206, 162, 0.4);"">
                                        <a href=""{_appUrl}/appointments"" 
                                           style=""display: inline-block; 
                                                  padding: 18px 50px; 
                                                  color: #ffffff; 
                                                  text-decoration: none; 
                                                  font-size: 16px; 
                                                  font-weight: 700;
                                                  letter-spacing: 0.5px;"">
                                            Randevularımı Görüntüle
                                        </a>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    
                    <!-- Footer -->
                    <tr>
                        <td style=""background: linear-gradient(135deg, #f5f7fa 0%, #c3cfe2 100%); padding: 35px 40px; border-top: 1px solid #e0e0e0;"">
                            <p style=""color: #78909c; font-size: 14px; line-height: 1.6; margin: 0 0 12px 0; text-align: center;"">
                                Sorularınız için bizimle iletişime geçmekten çekinmeyin.
                            </p>
                            <p style=""color: #90a4ae; font-size: 13px; margin: 0; text-align: center;"">
                                Bu otomatik bir emaildir, lütfen yanıtlamayın.
                            </p>
                            <p style=""color: #b0bec5; font-size: 12px; margin: 15px 0 0 0; text-align: center;"">
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
