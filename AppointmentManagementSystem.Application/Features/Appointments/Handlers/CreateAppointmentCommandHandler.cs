using AppointmentManagementSystem.Application.DTOs;
using AppointmentManagementSystem.Application.Features.Appointments.Commands;
using AppointmentManagementSystem.Domain.Entities;
using AppointmentManagementSystem.Domain.Interfaces;
using AutoMapper;
using MediatR;

namespace AppointmentManagementSystem.Application.Features.Appointments.Handlers
{
    public class CreateAppointmentCommandHandler : IRequestHandler<CreateAppointmentCommand, AppointmentDto>
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IServiceRepository _serviceRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreateAppointmentCommandHandler(
            IAppointmentRepository appointmentRepository,
            IServiceRepository serviceRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _appointmentRepository = appointmentRepository;
            _serviceRepository = serviceRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<AppointmentDto> Handle(CreateAppointmentCommand request, CancellationToken cancellationToken)
        {
            // Servis süresini al
            var service = await _serviceRepository.GetByIdAsync(request.CreateAppointmentDto.ServiceId);
            if (service == null)
                throw new Exception("Hizmet bulunamadı.");

            var appointment = _mapper.Map<Appointment>(request.CreateAppointmentDto);
            appointment.Status = "Pending";
            appointment.EndTime = appointment.StartTime.Add(TimeSpan.FromMinutes(service.DurationMinutes));

            await _appointmentRepository.AddAsync(appointment);
            await _unitOfWork.SaveChangesAsync();

            var appointmentWithDetails = await _appointmentRepository.GetByIdWithDetailsAsync(appointment.Id);
            return _mapper.Map<AppointmentDto>(appointmentWithDetails);
        }
    }
}
