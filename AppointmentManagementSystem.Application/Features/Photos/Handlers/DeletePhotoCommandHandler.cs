using AppointmentManagementSystem.Application.Features.Photos.Commands;
using AppointmentManagementSystem.Domain.Entities;
using AppointmentManagementSystem.Domain.Interfaces;
using MediatR;

namespace AppointmentManagementSystem.Application.Features.Photos.Handlers
{
    public class DeletePhotoCommandHandler : IRequestHandler<DeletePhotoCommand, bool>
    {
        private readonly IBusinessPhotoRepository _businessPhotoRepository;
        private readonly IEmployeePhotoRepository _employeePhotoRepository;
        private readonly IServicePhotoRepository _servicePhotoRepository;
        private readonly IAppointmentPhotoRepository _appointmentPhotoRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeletePhotoCommandHandler(
            IBusinessPhotoRepository businessPhotoRepository,
            IEmployeePhotoRepository employeePhotoRepository,
            IServicePhotoRepository servicePhotoRepository,
            IAppointmentPhotoRepository appointmentPhotoRepository,
            IUnitOfWork unitOfWork)
        {
            _businessPhotoRepository = businessPhotoRepository;
            _employeePhotoRepository = employeePhotoRepository;
            _servicePhotoRepository = servicePhotoRepository;
            _appointmentPhotoRepository = appointmentPhotoRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(DeletePhotoCommand request, CancellationToken cancellationToken)
        {
            // Try to find and delete photo from each repository (TPH - all in same table)
            // Try BusinessPhoto
            var businessPhoto = await _businessPhotoRepository.GetByIdAsync(request.PhotoId);
            if (businessPhoto != null)
            {
                businessPhoto.IsDeleted = true;
                await _unitOfWork.SaveChangesAsync();
                return true;
            }

            // Try EmployeePhoto
            var employeePhoto = await _employeePhotoRepository.GetByIdAsync(request.PhotoId);
            if (employeePhoto != null)
            {
                employeePhoto.IsDeleted = true;
                await _unitOfWork.SaveChangesAsync();
                return true;
            }

            // Try ServicePhoto
            var servicePhoto = await _servicePhotoRepository.GetByIdAsync(request.PhotoId);
            if (servicePhoto != null)
            {
                servicePhoto.IsDeleted = true;
                await _unitOfWork.SaveChangesAsync();
                return true;
            }

            // Try AppointmentPhoto
            var appointmentPhoto = await _appointmentPhotoRepository.GetByIdAsync(request.PhotoId);
            if (appointmentPhoto != null)
            {
                appointmentPhoto.IsDeleted = true;
                await _unitOfWork.SaveChangesAsync();
                return true;
            }

            // Photo not found
            return false;
        }
    }
}
