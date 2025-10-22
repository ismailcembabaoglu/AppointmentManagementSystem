using AppointmentManagementSystem.Application.DTOs;
using AppointmentManagementSystem.Application.Features.Services.Commands;
using AppointmentManagementSystem.Domain.Interfaces;
using AutoMapper;
using MediatR;

namespace AppointmentManagementSystem.Application.Features.Services.Handlers
{
    public class CreateServiceCommandHandler : IRequestHandler<CreateServiceCommand, ServiceDto>
    {
        private readonly IServiceRepository _serviceRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreateServiceCommandHandler(
            IServiceRepository serviceRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _serviceRepository = serviceRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ServiceDto> Handle(CreateServiceCommand request, CancellationToken cancellationToken)
        {
            var service = _mapper.Map<Domain.Entities.Service>(request.CreateServiceDto);
            await _serviceRepository.AddAsync(service);
            await _unitOfWork.SaveChangesAsync();

            var serviceWithBusiness = await _serviceRepository.GetByIdWithBusinessAsync(service.Id);
            return _mapper.Map<ServiceDto>(serviceWithBusiness);
        }
    }
}
