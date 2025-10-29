using AppointmentManagementSystem.Application.DTOs;
using AppointmentManagementSystem.Application.Features.Auth.Commands;
using AppointmentManagementSystem.Domain.Entities;
using AppointmentManagementSystem.Domain.Interfaces;
using MediatR;

namespace AppointmentManagementSystem.Application.Features.Auth.Handlers
{
    public class RegisterBusinessCommandHandler : IRequestHandler<RegisterBusinessCommand, AuthResponseDto>
    {
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<Business> _businessRepository;
        private readonly IRepository<Service> _serviceRepository;
        private readonly IRepository<Employee> _employeeRepository;
        private readonly IRepository<BusinessPhoto> _businessPhotoRepository;
        private readonly IRepository<EmployeePhoto> _employeePhotoRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IUnitOfWork _unitOfWork;

        public RegisterBusinessCommandHandler(
            IRepository<User> userRepository,
            IRepository<Category> categoryRepository,
            IRepository<Business> businessRepository,
            IRepository<Service> serviceRepository,
            IRepository<Employee> employeeRepository,
            IRepository<BusinessPhoto> businessPhotoRepository,
            IRepository<EmployeePhoto> employeePhotoRepository,
            IPasswordHasher passwordHasher,
            IJwtTokenGenerator jwtTokenGenerator,
            IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _categoryRepository = categoryRepository;
            _businessRepository = businessRepository;
            _serviceRepository = serviceRepository;
            _employeeRepository = employeeRepository;
            _businessPhotoRepository = businessPhotoRepository;
            _employeePhotoRepository = employeePhotoRepository;
            _passwordHasher = passwordHasher;
            _jwtTokenGenerator = jwtTokenGenerator;
            _unitOfWork = unitOfWork;
        }

        public async Task<AuthResponseDto> Handle(RegisterBusinessCommand request, CancellationToken cancellationToken)
        {
            var dto = request.RegisterBusinessDto;

            // Validate user doesn't exist
            var existingUser = await _userRepository.FindAsync(u => u.Email == dto.Email);
            if (existingUser.Any())
            {
                throw new Exception("Bu e-posta adresi zaten kullanımda.");
            }

            // Validate category exists
            var category = await _categoryRepository.GetByIdAsync(dto.CategoryId);
            if (category == null)
            {
                throw new Exception("Geçersiz kategori seçildi.");
            }

            // Validate at least one service
            if (!dto.Services.Any())
            {
                throw new Exception("En az bir hizmet eklemelisiniz.");
            }

            // Hash password
            var hashedPassword = _passwordHasher.HashPassword(dto.Password);

            // Create User
            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                PasswordHash = hashedPassword,
                Role = "Business"
            };

            await _userRepository.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            // Create Business
            var business = new Business
            {
                Name = dto.BusinessName,
                CategoryId = dto.CategoryId,
                Category = category,
                Description = dto.BusinessDescription,
                Address = dto.Address,
                Phone = dto.Phone,
                Email = dto.BusinessEmail,
                Website = dto.Website,
                City = dto.City,
                District = dto.District,
                Latitude = dto.Latitude,
                Longitude = dto.Longitude,
                IsActive = true
            };

            await _businessRepository.AddAsync(business);
            await _unitOfWork.SaveChangesAsync();

            // Update user with business ID
            user.OwnedBusinessId = business.Id;
            await _unitOfWork.SaveChangesAsync();

            // Add Business Photo if provided
            if (!string.IsNullOrEmpty(dto.BusinessPhotoBase64))
            {
                var businessPhoto = new BusinessPhoto
                {
                    BusinessId = business.Id,
                    FileName = $"business_{business.Id}_photo.jpg",
                    Base64Data = dto.BusinessPhotoBase64,
                    ContentType = "image/jpeg",
                    FileSize = dto.BusinessPhotoBase64.Length
                };
                await _businessPhotoRepository.AddAsync(businessPhoto);
            }

            // Create Services
            foreach (var serviceDto in dto.Services)
            {
                var service = new Service
                {
                    Name = serviceDto.Name,
                    Description = serviceDto.Description,
                    Price = serviceDto.Price,
                    DurationMinutes = serviceDto.DurationMinutes,
                    BusinessId = business.Id,
                    IsActive = true
                };
                await _serviceRepository.AddAsync(service);
            }

            // Save services and business photo
            await _unitOfWork.SaveChangesAsync();

            // Create Employees
            if (dto.Employees != null && dto.Employees.Any())
            {
                foreach (var employeeDto in dto.Employees)
                {
                    var employee = new Employee
                    {
                        Name = employeeDto.Name,
                        Specialization = employeeDto.Specialization,
                        Description = employeeDto.Description,
                        BusinessId = business.Id,
                        IsActive = employeeDto.IsActive
                    };
                    await _employeeRepository.AddAsync(employee);
                    await _unitOfWork.SaveChangesAsync(); // Save to get employee ID

                    // Add Employee Photos
                    if (employeeDto.PhotosBase64 != null && employeeDto.PhotosBase64.Any())
                    {
                        int photoIndex = 0;
                        foreach (var photoBase64 in employeeDto.PhotosBase64)
                        {
                            if (!string.IsNullOrEmpty(photoBase64))
                            {
                                photoIndex++;
                                var employeePhoto = new EmployeePhoto
                                {
                                    EmployeeId = employee.Id,
                                    FileName = $"employee_{employee.Id}_photo_{photoIndex}.jpg",
                                    Base64Data = photoBase64,
                                    ContentType = "image/jpeg",
                                    FileSize = photoBase64.Length
                                };
                                await _employeePhotoRepository.AddAsync(employeePhoto);
                            }
                        }
                    }
                }
            }

            // Final save for employee photos
            await _unitOfWork.SaveChangesAsync();

            // Generate token
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
