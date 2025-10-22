using AppointmentManagementSystem.Application.DTOs;
using AppointmentManagementSystem.Application.Features.Services.Commands;
using AppointmentManagementSystem.Domain.Interfaces;
using AutoMapper;
using MediatR;

namespace AppointmentManagementSystem.Application.Features.Services.Handlers
{
    public class UpdateServiceCommandHandler : IRequestHandler<UpdateServiceCommand, ServiceDto?>
    {
        private readonly IServiceRepository _serviceRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateServiceCommandHandler(
            IServiceRepository serviceRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _serviceRepository = serviceRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ServiceDto?> Handle(UpdateServiceCommand request, CancellationToken cancellationToken)
        {
            var service = await _serviceRepository.GetByIdAsync(request.Id);
            if (service == null)
                return null;

            _mapper.Map(request.CreateServiceDto, service);
            service.UpdatedAt = DateTime.UtcNow;

            await _serviceRepository.UpdateAsync(service);
            await _unitOfWork.SaveChangesAsync();

            var serviceWithBusiness = await _serviceRepository.GetByIdWithBusinessAsync(service.Id);
            return serviceWithBusiness != null ? _mapper.Map<ServiceDto>(serviceWithBusiness) : null;
        }
    }
}
