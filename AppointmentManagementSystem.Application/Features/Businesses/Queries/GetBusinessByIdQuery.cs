using AppointmentManagementSystem.Application.DTOs;
using MediatR;

namespace AppointmentManagementSystem.Application.Features.Businesses.Queries
{
    public class GetBusinessByIdQuery : IRequest<BusinessDto?>
    {
        public int Id { get; set; }
    }
}
