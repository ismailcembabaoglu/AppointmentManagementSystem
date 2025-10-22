using AppointmentManagementSystem.Application.Features.Services.Commands;
using AppointmentManagementSystem.Domain.Interfaces;
using MediatR;

namespace AppointmentManagementSystem.Application.Features.Services.Handlers
{
    public class DeleteServiceCommandHandler : IRequestHandler<DeleteServiceCommand, bool>
    {
        private readonly IServiceRepository _serviceRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteServiceCommandHandler(IServiceRepository serviceRepository, IUnitOfWork unitOfWork)
        {
            _serviceRepository = serviceRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(DeleteServiceCommand request, CancellationToken cancellationToken)
        {
            var service = await _serviceRepository.GetByIdAsync(request.Id);
            if (service == null)
                return false;

            service.IsDeleted = true;
            service.UpdatedAt = DateTime.UtcNow;

            await _serviceRepository.UpdateAsync(service);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
