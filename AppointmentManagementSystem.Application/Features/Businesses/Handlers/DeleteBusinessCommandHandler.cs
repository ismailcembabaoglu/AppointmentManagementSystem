using AppointmentManagementSystem.Application.Features.Businesses.Commands;
using AppointmentManagementSystem.Domain.Interfaces;
using MediatR;

namespace AppointmentManagementSystem.Application.Features.Businesses.Handlers
{
    public class DeleteBusinessCommandHandler : IRequestHandler<DeleteBusinessCommand, bool>
    {
        private readonly IBusinessRepository _businessRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteBusinessCommandHandler(IBusinessRepository businessRepository, IUnitOfWork unitOfWork)
        {
            _businessRepository = businessRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(DeleteBusinessCommand request, CancellationToken cancellationToken)
        {
            var business = await _businessRepository.GetByIdAsync(request.Id);
            if (business == null)
                return false;

            business.IsDeleted = true;
            business.UpdatedAt = DateTime.UtcNow;

            await _businessRepository.UpdateAsync(business);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
