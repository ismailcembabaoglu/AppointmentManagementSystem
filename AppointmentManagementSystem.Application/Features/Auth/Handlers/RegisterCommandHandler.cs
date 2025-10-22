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
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, BaseResponse<AuthResponseDto>> // DÖNÜŞ TİPİ DEĞİŞTİ
    {
        private readonly IRepository<User> _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<RegisterDto> _validator; // VALIDATOR EKLE

        public RegisterCommandHandler(
            IRepository<User> userRepository,
            IPasswordHasher passwordHasher,
            IJwtTokenGenerator jwtTokenGenerator,
            IUnitOfWork unitOfWork,
            IValidator<RegisterDto> validator) // PARAMETRE EKLE
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _jwtTokenGenerator = jwtTokenGenerator;
            _unitOfWork = unitOfWork;
            _validator = validator;
        }

        public async Task<BaseResponse<AuthResponseDto>> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            // Validation
            var validationResult = await _validator.ValidateAsync(request.RegisterDto, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return BaseResponse<AuthResponseDto>.ValidationErrorResponse(errors);
            }

            try
            {
                // Kullanıcı zaten var mı kontrol et
                var existingUser = await _userRepository.FindAsync(u => u.Email == request.RegisterDto.Email);
                if (existingUser.Any())
                {
                    return BaseResponse<AuthResponseDto>.ErrorResponse("Bu e-posta adresi zaten kullanımda.");
                }

                // Şifre hashle
                var hashedPassword = _passwordHasher.HashPassword(request.RegisterDto.Password);

                // Yeni kullanıcı oluştur
                var user = new User
                {
                    Name = request.RegisterDto.Name,
                    Email = request.RegisterDto.Email,
                    PasswordHash = hashedPassword,
                    Role = request.RegisterDto.Role
                };

                await _userRepository.AddAsync(user);
                await _unitOfWork.SaveChangesAsync();

                // Token oluştur
                var token = _jwtTokenGenerator.GenerateToken(user);

                var authResponse = new AuthResponseDto
                {
                    Token = token,
                    UserName = user.Name,
                    Role = user.Role,
                    ExpiresAt = DateTime.UtcNow.AddHours(24)
                };

                return BaseResponse<AuthResponseDto>.SuccessResponse(authResponse, "Kayıt işlemi başarılı.");
            }
            catch (Exception ex)
            {
                return BaseResponse<AuthResponseDto>.ErrorResponse($"Bir hata oluştu: {ex.Message}");
            }
        }
    }
}
