using AppointmentManagementSystem.Application.Features.Admin.Queries;
using AppointmentManagementSystem.Domain.Interfaces;
using MediatR;
using System.Linq;

namespace AppointmentManagementSystem.Application.Features.Admin.Handlers
{
    public class GetBusinessAppointmentsAdminHandler : IRequestHandler<GetBusinessAppointmentsAdminQuery, List<AppointmentAdminDto>>
    {
        private readonly IAppointmentRepository _appointmentRepository;

        public GetBusinessAppointmentsAdminHandler(IAppointmentRepository appointmentRepository)
        {
            _appointmentRepository = appointmentRepository;
        }

        public async Task<List<AppointmentAdminDto>> Handle(GetBusinessAppointmentsAdminQuery request, CancellationToken cancellationToken)
        {
            var appointments = await _appointmentRepository.GetAllWithDetailsAsync(businessId: request.BusinessId);

            var query = appointments.AsEnumerable();

            if (request.StartDate.HasValue)
            {
                query = query.Where(a => a.AppointmentDate >= request.StartDate.Value);
            }

            if (request.EndDate.HasValue)
            {
                query = query.Where(a => a.AppointmentDate <= request.EndDate.Value);
            }

            if (!string.IsNullOrEmpty(request.Status))
            {
                query = query.Where(a => a.Status == request.Status);
            }

            return query.Select(a => new AppointmentAdminDto
            {
                Id = a.Id,
                AppointmentDate = a.AppointmentDate,
                StartTime = a.StartTime,
                EndTime = a.EndTime,
                Status = a.Status,
                CustomerName = a.Customer?.Name ?? "N/A",
                CustomerEmail = a.Customer?.Email,
                CustomerPhone = a.Customer?.PhoneNumber,
                ServiceName = a.AppointmentServices.Any()
                    ? string.Join(", ", a.AppointmentServices.Select(s => s.ServiceName))
                    : a.Service?.Name ?? "N/A",
                ServicePrice = a.AppointmentServices.Any()
                    ? a.AppointmentServices.Sum(s => s.Price)
                    : a.Service?.Price,
                EmployeeName = a.Employee?.Name,
                Notes = a.Notes,
                Rating = a.Rating,
                Review = a.Review,
                CreatedAt = a.CreatedAt
            }).OrderByDescending(a => a.AppointmentDate).ToList();
        }
    }
}