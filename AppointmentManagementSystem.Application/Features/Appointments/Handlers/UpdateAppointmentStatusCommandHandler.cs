using AppointmentManagementSystem.Application.DTOs;
using AppointmentManagementSystem.Application.Features.Appointments.Commands;
using AppointmentManagementSystem.Domain.Interfaces;
using AutoMapper;
using MediatR;

namespace AppointmentManagementSystem.Application.Features.Appointments.Handlers
{
    public class UpdateAppointmentStatusCommandHandler : IRequestHandler<UpdateAppointmentStatusCommand, AppointmentDto?>
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateAppointmentStatusCommandHandler(
            IAppointmentRepository appointmentRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _appointmentRepository = appointmentRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<AppointmentDto?> Handle(UpdateAppointmentStatusCommand request, CancellationToken cancellationToken)
        {
            await _appointmentRepository.UpdateStatusAsync(request.Id, request.Status);
            await _unitOfWork.SaveChangesAsync();

            var appointment = await _appointmentRepository.GetByIdWithDetailsAsync(request.Id);
            return appointment != null ? _mapper.Map<AppointmentDto>(appointment) : null;
        }
    }
}
