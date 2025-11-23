using AppointmentManagementSystem.Application.Features.Admin.Commands;
using AppointmentManagementSystem.Domain.Interfaces;
using MediatR;

namespace AppointmentManagementSystem.Application.Features.Admin.Handlers
{
    public class UpdateBusinessStatusHandler : IRequestHandler<UpdateBusinessStatusCommand, bool>
    {
        private readonly IBusinessRepository _businessRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateBusinessStatusHandler(
            IBusinessRepository businessRepository,
            IUnitOfWork unitOfWork)
        {
            _businessRepository = businessRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(UpdateBusinessStatusCommand request, CancellationToken cancellationToken)
        {
            var business = await _businessRepository.GetByIdAsync(request.BusinessId);
            if (business == null)
                return false;

            business.IsActive = request.IsActive;
            business.UpdatedAt = DateTime.UtcNow;

            await _businessRepository.UpdateAsync(business);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}