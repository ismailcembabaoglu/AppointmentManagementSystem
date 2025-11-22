using AppointmentManagementSystem.Application.DTOs;
using AppointmentManagementSystem.Application.Models;
using AppointmentManagementSystem.Application.Shared;
using MediatR;

namespace AppointmentManagementSystem.Application.Features.Appointments.Queries
{
    public class GetAllAppointmentsQuery : PaginationParameters, IRequest<PaginatedResult<AppointmentDto>>
    {
        public int? CustomerId { get; set; }
        public int? BusinessId { get; set; }
        public string? Status { get; set; }
        public string? SortBy { get; set; }
    }
}
