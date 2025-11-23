using AppointmentManagementSystem.Application.Features.Admin.Commands;
using AppointmentManagementSystem.Domain.Interfaces;
using MediatR;

namespace AppointmentManagementSystem.Application.Features.Admin.Handlers
{
    public class UpdateSubscriptionAutoRenewalHandler : IRequestHandler<UpdateSubscriptionAutoRenewalCommand, bool>
    {
        private readonly IBusinessSubscriptionRepository _subscriptionRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateSubscriptionAutoRenewalHandler(
            IBusinessSubscriptionRepository subscriptionRepository,
            IUnitOfWork unitOfWork)
        {
            _subscriptionRepository = subscriptionRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(UpdateSubscriptionAutoRenewalCommand request, CancellationToken cancellationToken)
        {
            var subscription = await _subscriptionRepository.GetByBusinessIdAsync(request.BusinessId);
            if (subscription == null)
                return false;

            subscription.AutoRenewal = request.AutoRenewal;
            subscription.UpdatedAt = DateTime.UtcNow;

            await _subscriptionRepository.UpdateAsync(subscription);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}