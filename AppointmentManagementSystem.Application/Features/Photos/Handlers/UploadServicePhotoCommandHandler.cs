using AppointmentManagementSystem.Application.DTOs;
using AppointmentManagementSystem.Application.Features.Photos.Commands;
using AppointmentManagementSystem.Domain.Entities;
using AppointmentManagementSystem.Domain.Interfaces;
using AutoMapper;
using MediatR;

namespace AppointmentManagementSystem.Application.Features.Photos.Handlers
{
    public class UploadServicePhotoCommandHandler : IRequestHandler<UploadServicePhotoCommand, PhotoDto>
    {
        private readonly IServicePhotoRepository _photoRepository;
        private readonly IServiceRepository _serviceRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IImageOptimizationService _imageOptimizationService;

        public UploadServicePhotoCommandHandler(
            IServicePhotoRepository photoRepository,
            IServiceRepository serviceRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IImageOptimizationService imageOptimizationService)
        {
            _photoRepository = photoRepository;
            _serviceRepository = serviceRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _imageOptimizationService = imageOptimizationService;
        }

        public async Task<PhotoDto> Handle(UploadServicePhotoCommand request, CancellationToken cancellationToken)
        {
            var service = await _serviceRepository.GetByIdAsync(request.ServiceId);
            if (service == null)
                throw new Exception($"Service with ID {request.ServiceId} not found.");

            if (string.IsNullOrEmpty(request.PhotoDto.Base64Data))
                throw new Exception("Base64 data is required.");

            var optimized = _imageOptimizationService.OptimizeImage(request.PhotoDto.Base64Data, request.PhotoDto.ContentType);

            if (optimized.FileSize > 10 * 1024 * 1024)
                throw new Exception("File size exceeds 10MB limit.");

            var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/webp" };
            if (!allowedTypes.Contains(optimized.ContentType.ToLower()))
                throw new Exception("Invalid file type. Only JPEG, PNG, and WebP are allowed.");

            var photo = new ServicePhoto
            {
                ServiceId = request.ServiceId,
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
