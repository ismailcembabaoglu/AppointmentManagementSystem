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
        private readonly IBusinessRepository _businessRepository; // BU SATIR EKLENDİ
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreateServiceCommandHandler(
            IServiceRepository serviceRepository,
            IBusinessRepository businessRepository, // BU SATIR EKLENDİ
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _serviceRepository = serviceRepository;
            _businessRepository = businessRepository; // BU SATIR EKLENDİ
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ServiceDto> Handle(CreateServiceCommand request, CancellationToken cancellationToken)
        {
            // Business kontrolü
            var business = await _businessRepository.GetByIdAsync(request.CreateServiceDto.BusinessId);
            if (business == null)
            {
                throw new Exception($"Business with ID {request.CreateServiceDto.BusinessId} not found.");
            }

            var service = _mapper.Map<Domain.Entities.Service>(request.CreateServiceDto);
            await _serviceRepository.AddAsync(service);
            await _unitOfWork.SaveChangesAsync();

            var serviceWithBusiness = await _serviceRepository.GetByIdWithBusinessAsync(service.Id);
            return _mapper.Map<ServiceDto>(serviceWithBusiness);
        }
    }
}
