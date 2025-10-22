using MediatR;

namespace AppointmentManagementSystem.Application.Features.Employees.Commands
{
    public class DeleteEmployeeCommand : IRequest<bool>
    {
        public int Id { get; set; }
    }
}
