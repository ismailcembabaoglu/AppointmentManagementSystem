using MediatR;

namespace AppointmentManagementSystem.Application.Features.Admin.Commands
{
    public class DeleteEmployeeAdminCommand : IRequest<bool>
    {
        public int EmployeeId { get; set; }
    }
}