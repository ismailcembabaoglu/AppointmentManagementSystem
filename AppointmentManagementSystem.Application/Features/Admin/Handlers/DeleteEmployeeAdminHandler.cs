using AppointmentManagementSystem.Application.Features.Admin.Commands;
using AppointmentManagementSystem.Domain.Interfaces;
using MediatR;

namespace AppointmentManagementSystem.Application.Features.Admin.Handlers
{
    public class DeleteEmployeeAdminHandler : IRequestHandler<DeleteEmployeeAdminCommand, bool>
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteEmployeeAdminHandler(
            IEmployeeRepository employeeRepository,
            IUnitOfWork unitOfWork)
        {
            _employeeRepository = employeeRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(DeleteEmployeeAdminCommand request, CancellationToken cancellationToken)
        {
            var employee = await _employeeRepository.GetByIdAsync(request.EmployeeId);
            if (employee == null)
                return false;

            await _employeeRepository.DeleteAsync(request.EmployeeId);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}