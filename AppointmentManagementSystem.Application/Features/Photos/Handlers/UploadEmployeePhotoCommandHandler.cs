using AppointmentManagementSystem.Application.DTOs;
using AppointmentManagementSystem.Application.Features.Photos.Commands;
using AppointmentManagementSystem.Domain.Entities;
using AppointmentManagementSystem.Domain.Interfaces;
using AutoMapper;
using MediatR;

namespace AppointmentManagementSystem.Application.Features.Photos.Handlers
{
    public class UploadEmployeePhotoCommandHandler : IRequestHandler<UploadEmployeePhotoCommand, PhotoDto>
    {
        private readonly IEmployeePhotoRepository _photoRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IImageOptimizationService _imageOptimizationService;

        public UploadEmployeePhotoCommandHandler(
            IEmployeePhotoRepository photoRepository,
            IEmployeeRepository employeeRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IImageOptimizationService imageOptimizationService)
        {
            _photoRepository = photoRepository;
            _employeeRepository = employeeRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _imageOptimizationService = imageOptimizationService;
        }

        public async Task<PhotoDto> Handle(UploadEmployeePhotoCommand request, CancellationToken cancellationToken)
        {
            var employee = await _employeeRepository.GetByIdAsync(request.EmployeeId);
            if (employee == null)
                throw new Exception($"Employee with ID {request.EmployeeId} not found.");

            if (string.IsNullOrEmpty(request.PhotoDto.Base64Data))
                throw new Exception("Base64 data is required.");

            var optimized = _imageOptimizationService.OptimizeImage(request.PhotoDto.Base64Data, request.PhotoDto.ContentType);

            if (optimized.FileSize > 10 * 1024 * 1024)
                throw new Exception("File size exceeds 10MB limit.");

            var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/webp" };
            if (!allowedTypes.Contains(optimized.ContentType.ToLower()))
                throw new Exception("Invalid file type. Only JPEG, PNG, and WebP are allowed.");

            var photo = new EmployeePhoto
            {
                EmployeeId = request.EmployeeId,
                FileName = request.PhotoDto.FileName,
                ContentType = optimized.ContentType,
                FileSize = optimized.FileSize,
                Base64Data = optimized.Base64Data,
                FilePath = null
            };

            await _photoRepository.AddAsync(photo);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<PhotoDto>(photo);
        }
    }
}
