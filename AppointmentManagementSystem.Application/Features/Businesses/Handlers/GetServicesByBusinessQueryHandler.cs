using AppointmentManagementSystem.Application.DTOs;
using AppointmentManagementSystem.Application.Features.Businesses.Queries;
using AppointmentManagementSystem.Domain.Interfaces;
using AutoMapper;
using MediatR;

namespace AppointmentManagementSystem.Application.Features.Businesses.Handlers
{
    public class GetServicesByBusinessQueryHandler : IRequestHandler<GetServicesByBusinessQuery, List<ServiceDto>>
    {
        private readonly IServiceRepository _serviceRepository;
        private readonly IMapper _mapper;

        public GetServicesByBusinessQueryHandler(IServiceRepository serviceRepository, IMapper mapper)
        {
            _serviceRepository = serviceRepository;
            _mapper = mapper;
        }

        public async Task<List<ServiceDto>> Handle(GetServicesByBusinessQuery request, CancellationToken cancellationToken)
        {
            var services = await _serviceRepository.GetByBusinessAsync(request.BusinessId);
            return _mapper.Map<List<ServiceDto>>(services);
        }
    }
}
