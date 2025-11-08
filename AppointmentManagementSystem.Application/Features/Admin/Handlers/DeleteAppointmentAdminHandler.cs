using AppointmentManagementSystem.Application.Features.Admin.Commands;
using AppointmentManagementSystem.Domain.Interfaces;
using MediatR;

namespace AppointmentManagementSystem.Application.Features.Admin.Handlers
{
    public class DeleteAppointmentAdminHandler : IRequestHandler<DeleteAppointmentAdminCommand, bool>
    {
        private readonly IAppointmentRepository _appointmentRepository;

        public DeleteAppointmentAdminHandler(IAppointmentRepository appointmentRepository)
        {
            _appointmentRepository = appointmentRepository;
        }

        public async Task<bool> Handle(DeleteAppointmentAdminCommand request, CancellationToken cancellationToken)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(request.AppointmentId);
            if (appointment == null)
                return false;

            await _appointmentRepository.DeleteAsync(request.AppointmentId);
            return true;
        }
    }
}