using AppointmentManagementSystem.Application.DTOs;
using MediatR;

namespace AppointmentManagementSystem.Application.Features.Businesses.Queries
{
    public class GetEmployeesByBusinessQuery : IRequest<List<EmployeeDto>>
    {
        public int BusinessId { get; set; }
    }
}
