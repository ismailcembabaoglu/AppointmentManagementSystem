using AppointmentManagementSystem.Application.Features.Admin.Commands;
using AppointmentManagementSystem.Domain.Interfaces;
using MediatR;

namespace AppointmentManagementSystem.Application.Features.Admin.Handlers
{
    public class UpdateAppointmentStatusAdminHandler : IRequestHandler<UpdateAppointmentStatusAdminCommand, bool>
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateAppointmentStatusAdminHandler(
            IAppointmentRepository appointmentRepository,
            IUnitOfWork unitOfWork)
        {
            _appointmentRepository = appointmentRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(UpdateAppointmentStatusAdminCommand request, CancellationToken cancellationToken)
        {
            await _appointmentRepository.UpdateStatusAsync(request.AppointmentId, request.Status);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}