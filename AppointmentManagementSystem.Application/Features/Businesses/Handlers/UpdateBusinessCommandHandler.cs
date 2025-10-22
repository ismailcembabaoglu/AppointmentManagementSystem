using AppointmentManagementSystem.Application.DTOs;
using AppointmentManagementSystem.Application.Features.Businesses.Commands;
using AppointmentManagementSystem.Domain.Interfaces;
using AutoMapper;
using MediatR;

namespace AppointmentManagementSystem.Application.Features.Businesses.Handlers
{
    public class UpdateBusinessCommandHandler : IRequestHandler<UpdateBusinessCommand, BusinessDto?>
    {
        private readonly IBusinessRepository _businessRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateBusinessCommandHandler(
            IBusinessRepository businessRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _businessRepository = businessRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<BusinessDto?> Handle(UpdateBusinessCommand request, CancellationToken cancellationToken)
        {
            var business = await _businessRepository.GetByIdAsync(request.Id);
            if (business == null)
                return null;

            _mapper.Map(request.CreateBusinessDto, business);
            business.UpdatedAt = DateTime.UtcNow;

            await _businessRepository.UpdateAsync(business);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<BusinessDto>(business);
        }
    }
}
