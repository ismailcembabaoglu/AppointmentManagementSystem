using AppointmentManagementSystem.Application.Features.Admin.Commands;
using AppointmentManagementSystem.Domain.Interfaces;
using MediatR;

namespace AppointmentManagementSystem.Application.Features.Admin.Handlers
{
    public class DeleteAppointmentAdminHandler : IRequestHandler<DeleteAppointmentAdminCommand, bool>
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteAppointmentAdminHandler(
            IAppointmentRepository appointmentRepository,
            IUnitOfWork unitOfWork)
        {
            _appointmentRepository = appointmentRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(DeleteAppointmentAdminCommand request, CancellationToken cancellationToken)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(request.AppointmentId);
            if (appointment == null)
                return false;

            await _appointmentRepository.DeleteAsync(request.AppointmentId);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}