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
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResponseDto> // DÖNÜŞ TİPİ DEĞİŞTİ
    {
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Category> _CategoryRepository; // EKLENDİ
        private readonly IRepository<Business> _BusinessRepository; // EKLENDİ
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<RegisterDto> _validator; // VALIDATOR EKLE
        private readonly IEmailService _emailService;

        public RegisterCommandHandler(
            IRepository<User> userRepository,
            IPasswordHasher passwordHasher,
            IJwtTokenGenerator jwtTokenGenerator,
            IUnitOfWork unitOfWork,
            IValidator<RegisterDto> validator, 
            IRepository<Category> categoryRepository, 
            IRepository<Business> businessRepository,
            IEmailService emailService) // PARAMETRE EKLE
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _jwtTokenGenerator = jwtTokenGenerator;
            _unitOfWork = unitOfWork;
            _validator = validator;
            _CategoryRepository = categoryRepository; // ATA
            _BusinessRepository = businessRepository;
            _emailService = emailService;
        }

        public async Task<AuthResponseDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            // Validation
            var validationResult = await _validator.ValidateAsync(request.RegisterDto, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                throw new ValidationException(string.Join(", ", errors));
            }

            // Kullanıcı zaten var mı kontrol et
            var existingUser = await _userRepository.FindAsync(u => u.Email == request.RegisterDto.Email);
            if (existingUser.Any())
            {
                throw new Exception("Bu e-posta adresi zaten kullanımda.");
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

            // Business rolü için business oluştur
            if (request.RegisterDto.Role == "Business" && request.RegisterDto.BusinessInfo != null)
            {
                // Category kontrolü
                var category = await _CategoryRepository.GetByIdAsync(request.RegisterDto.BusinessInfo.CategoryId);
                    

                if (category == null)
                {
                    throw new Exception("Geçersiz kategori seçildi.");
                }

                var business = new Business
                {
                    Name = request.RegisterDto.BusinessInfo.Name,
                    CategoryId = category.Id,
                    Category=category,
                    Description = request.RegisterDto.BusinessInfo.Description,
                    Address = request.RegisterDto.BusinessInfo.Address,
                    Phone = request.RegisterDto.BusinessInfo.Phone,
                    Email = request.RegisterDto.BusinessInfo.Email,
                    Website = request.RegisterDto.BusinessInfo.Website,
                    City = request.RegisterDto.BusinessInfo.City,
                    District = request.RegisterDto.BusinessInfo.District,
                    Latitude = request.RegisterDto.BusinessInfo.Latitude,
                    Longitude = request.RegisterDto.BusinessInfo.Longitude,
                    IsActive = true
                };

                await _BusinessRepository.AddAsync(business);
                await _unitOfWork.SaveChangesAsync();

                // User'a business ID ata
                user.OwnedBusinessId = business.Id;
            }

            await _userRepository.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            // Token oluştur
            var token = _jwtTokenGenerator.GenerateToken(user);

            return new AuthResponseDto
            {
                Token = token,
                UserName = user.Name,
                Role = user.Role,
                ExpiresAt = DateTime.UtcNow.AddHours(24)
            };
        }

    }
}
