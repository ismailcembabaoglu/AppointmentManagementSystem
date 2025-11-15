using AppointmentManagementSystem.Application.Features.Auth.Commands;
using AppointmentManagementSystem.Domain.Entities;
using AppointmentManagementSystem.Domain.Interfaces;
using MediatR;

namespace AppointmentManagementSystem.Application.Features.Auth.Handlers
{
    public class VerifyEmailCommandHandler : IRequestHandler<VerifyEmailCommand, bool>
    {
        private readonly IRepository<User> _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;

        public VerifyEmailCommandHandler(
            IRepository<User> userRepository,
            IUnitOfWork unitOfWork,
            IEmailService emailService)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _emailService = emailService;
        }

        public async Task<bool> Handle(VerifyEmailCommand request, CancellationToken cancellationToken)
        {
            // Token ile kullanıcıyı bul
            var users = await _userRepository.FindAsync(u => u.EmailVerificationToken == request.Token);
            var user = users.FirstOrDefault();

            if (user == null)
            {
                throw new Exception("Geçersiz doğrulama linki.");
            }

            // Token süresi dolmuş mu kontrol et
            if (user.EmailVerificationTokenExpiry == null || user.EmailVerificationTokenExpiry < DateTime.UtcNow)
            {
                throw new Exception("Doğrulama linkinin süresi dolmuş. Lütfen yeni bir link talep edin.");
            }

            // Kullanıcıyı aktif et
            user.IsActive = true;
            user.IsEmailVerified = true;
            user.EmailVerificationToken = null;
            user.EmailVerificationTokenExpiry = null;

            await _unitOfWork.SaveChangesAsync();

            // Hoş geldin emaili gönder
            try
            {
                await _emailService.SendWelcomeEmailAsync(user.Email, user.Name);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hoş geldin emaili gönderilemedi: {ex.Message}");
            }

            return true;
        }
    }
}
