using AppointmentManagementSystem.Application.DTOs;
using AppointmentManagementSystem.Application.Features.Businesses.Queries;
using AppointmentManagementSystem.Domain.Interfaces;
using AutoMapper;
using MediatR;

namespace AppointmentManagementSystem.Application.Features.Businesses.Handlers
{
    public class GetAppointmentsByBusinessQueryHandler : IRequestHandler<GetAppointmentsByBusinessQuery, List<AppointmentDto>>
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IMapper _mapper;

        public GetAppointmentsByBusinessQueryHandler(IAppointmentRepository appointmentRepository, IMapper mapper)
        {
            _appointmentRepository = appointmentRepository;
            _mapper = mapper;
        }

        public async Task<List<AppointmentDto>> Handle(GetAppointmentsByBusinessQuery request, CancellationToken cancellationToken)
        {
            var appointments = await _appointmentRepository.GetByBusinessAsync(request.BusinessId);
            return _mapper.Map<List<AppointmentDto>>(appointments);
        }
    }
}
