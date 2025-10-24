using AppointmentManagementSystem.Application.DTOs;
using AppointmentManagementSystem.Application.Features.Photos.Commands;
using AppointmentManagementSystem.Domain.Entities;
using AppointmentManagementSystem.Domain.Interfaces;
using AutoMapper;
using MediatR;

namespace AppointmentManagementSystem.Application.Features.Photos.Handlers
{
    public class UploadBusinessPhotoCommandHandler : IRequestHandler<UploadBusinessPhotoCommand, PhotoDto>
    {
        private readonly IBusinessPhotoRepository _photoRepository;
        private readonly IBusinessRepository _businessRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UploadBusinessPhotoCommandHandler(
            IBusinessPhotoRepository photoRepository,
            IBusinessRepository businessRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _photoRepository = photoRepository;
            _businessRepository = businessRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PhotoDto> Handle(UploadBusinessPhotoCommand request, CancellationToken cancellationToken)
        {
            // Validate business exists
            var business = await _businessRepository.GetByIdAsync(request.BusinessId);
            if (business == null)
                throw new Exception($"Business with ID {request.BusinessId} not found.");

            // Validate file data
            if (string.IsNullOrEmpty(request.PhotoDto.Base64Data))
                throw new Exception("Base64 data is required.");

            // Calculate file size from base64 string
            var base64Length = request.PhotoDto.Base64Data.Length;
            var fileSize = (long)(base64Length * 0.75); // Approximate size in bytes

            // Validate file size (max 10MB for images)
            if (fileSize > 10 * 1024 * 1024)
                throw new Exception("File size exceeds 10MB limit.");

            // Validate content type
            var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/webp" };
            if (!string.IsNullOrEmpty(request.PhotoDto.ContentType) &&
                !allowedTypes.Contains(request.PhotoDto.ContentType.ToLower()))
                throw new Exception("Invalid file type. Only JPEG, PNG, and WebP are allowed.");

            // Create photo entity
            var photo = new BusinessPhoto
            {
                BusinessId = request.BusinessId,
                FileName = request.PhotoDto.FileName,
                ContentType = request.PhotoDto.ContentType,
                FileSize = fileSize,
                Base64Data = request.PhotoDto.Base64Data,
                FilePath = null // Not using file system
            };

            await _photoRepository.AddAsync(photo);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<PhotoDto>(photo);
        }
    }
}
