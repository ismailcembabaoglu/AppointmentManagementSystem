using AppointmentManagementSystem.Application.Features.Admin.Commands;
using AppointmentManagementSystem.Domain.Interfaces;
using MediatR;

namespace AppointmentManagementSystem.Application.Features.Admin.Handlers
{
    public class UpdateAppointmentStatusAdminHandler : IRequestHandler<UpdateAppointmentStatusAdminCommand, bool>
    {
        private readonly IAppointmentRepository _appointmentRepository;

        public UpdateAppointmentStatusAdminHandler(IAppointmentRepository appointmentRepository)
        {
            _appointmentRepository = appointmentRepository;
        }

        public async Task<bool> Handle(UpdateAppointmentStatusAdminCommand request, CancellationToken cancellationToken)
        {
            await _appointmentRepository.UpdateStatusAsync(request.AppointmentId, request.Status);
            return true;
        }
    }
}