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

        public UploadServicePhotoCommandHandler(
            IServicePhotoRepository photoRepository,
            IServiceRepository serviceRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _photoRepository = photoRepository;
            _serviceRepository = serviceRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PhotoDto> Handle(UploadServicePhotoCommand request, CancellationToken cancellationToken)
        {
            var service = await _serviceRepository.GetByIdAsync(request.ServiceId);
            if (service == null)
                throw new Exception($"Service with ID {request.ServiceId} not found.");

            if (string.IsNullOrEmpty(request.PhotoDto.Base64Data))
                throw new Exception("Base64 data is required.");

            var base64Length = request.PhotoDto.Base64Data.Length;
            var fileSize = (long)(base64Length * 0.75);

            if (fileSize > 10 * 1024 * 1024)
                throw new Exception("File size exceeds 10MB limit.");

            var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/webp" };
            if (!string.IsNullOrEmpty(request.PhotoDto.ContentType) &&
                !allowedTypes.Contains(request.PhotoDto.ContentType.ToLower()))
                throw new Exception("Invalid file type. Only JPEG, PNG, and WebP are allowed.");

            var photo = new ServicePhoto
            {
                ServiceId = request.ServiceId,
                FileName = request.PhotoDto.FileName,
                ContentType = request.PhotoDto.ContentType,
                FileSize = fileSize,
                Base64Data = request.PhotoDto.Base64Data,
                FilePath = null
            };

            await _photoRepository.AddAsync(photo);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<PhotoDto>(photo);
        }
    }
}
