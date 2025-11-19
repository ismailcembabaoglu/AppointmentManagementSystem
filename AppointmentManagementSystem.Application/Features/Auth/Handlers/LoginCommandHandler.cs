using AppointmentManagementSystem.Application.DTOs;
using AppointmentManagementSystem.Application.Features.Auth.Commands;
using AppointmentManagementSystem.Application.Shared; // BU SATIRI EKLE
using AppointmentManagementSystem.Application.Validators; // BU SATIRI EKLE
using AppointmentManagementSystem.Domain.Entities;
using AppointmentManagementSystem.Domain.Interfaces;
using FluentValidation; // BU SATIRI EKLE
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AppointmentManagementSystem.Application.Features.Auth.Handlers
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, BaseResponse<AuthResponseDto>> // DÖNÜŞ TİPİ DEĞİŞTİ
    {
        private readonly IRepository<User> _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<LoginDto> _validator; // VALIDATOR EKLE

        public LoginCommandHandler(
            IRepository<User> userRepository,
            IPasswordHasher passwordHasher,
            IJwtTokenGenerator jwtTokenGenerator,
            IUnitOfWork unitOfWork,
            IValidator<LoginDto> validator) // PARAMETRE EKLE
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _jwtTokenGenerator = jwtTokenGenerator;
            _unitOfWork = unitOfWork;
            _validator = validator;
        }

        public async Task<BaseResponse<AuthResponseDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            // Validation
            var validationResult = await _validator.ValidateAsync(request.LoginDto, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return BaseResponse<AuthResponseDto>.ValidationErrorResponse(errors);
            }

            try
            {
                // Kullanıcıyı e-posta ile bul
                var user = (await _userRepository.FindAsync(u => u.Email == request.LoginDto.Email)).FirstOrDefault();

                if (user == null)
                {
                    return BaseResponse<AuthResponseDto>.ErrorResponse("Geçersiz e-posta veya şifre.");
                }

                // Şifre kontrolü
                if (!_passwordHasher.VerifyPassword(request.LoginDto.Password, user.PasswordHash))
                {
                    return BaseResponse<AuthResponseDto>.ErrorResponse("Geçersiz e-posta veya şifre.");
                }
                if (!user.IsEmailVerified)
                {
                    return BaseResponse<AuthResponseDto>.ErrorResponse("Lütfen Hesabınızı Aktifleştirin");
                }
                // Token oluştur
                var token = _jwtTokenGenerator.GenerateToken(user);

                var authResponse = new AuthResponseDto
                {
                    Token = token,
                    UserName = user.Name,
                    Role = user.Role,
                    ExpiresAt = DateTime.UtcNow.AddHours(24)
                };

                return BaseResponse<AuthResponseDto>.SuccessResponse(authResponse, "Giriş başarılı.");
            }
            catch (Exception ex)
            {
                return BaseResponse<AuthResponseDto>.ErrorResponse($"Bir hata oluştu: {ex.Message}");
            }
        }
    }
}
