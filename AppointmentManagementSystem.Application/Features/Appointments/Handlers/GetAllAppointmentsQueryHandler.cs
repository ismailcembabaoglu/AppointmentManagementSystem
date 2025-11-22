using AppointmentManagementSystem.Application.DTOs;
using AppointmentManagementSystem.Application.Features.Appointments.Queries;
using AppointmentManagementSystem.Application.Shared;
using AppointmentManagementSystem.Domain.Interfaces;
using AutoMapper;
using MediatR;
using System.Collections.Generic;
using System.Linq;

namespace AppointmentManagementSystem.Application.Features.Appointments.Handlers
{
    public class GetAllAppointmentsQueryHandler : IRequestHandler<GetAllAppointmentsQuery, PaginatedResult<AppointmentDto>>
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IMapper _mapper;

        public GetAllAppointmentsQueryHandler(IAppointmentRepository appointmentRepository, IMapper mapper)
        {
            _appointmentRepository = appointmentRepository;
            _mapper = mapper;
        }

        public async Task<PaginatedResult<AppointmentDto>> Handle(GetAllAppointmentsQuery request, CancellationToken cancellationToken)
        {
            var appointments = await _appointmentRepository.GetAllWithDetailsAsync(request.CustomerId, request.BusinessId);
            var filteredAppointments = appointments.ToList();

            if (!string.IsNullOrEmpty(request.Status) && request.Status != "All")
            {
                filteredAppointments = filteredAppointments
                    .Where(a => a.Status.Equals(request.Status, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            filteredAppointments = request.SortBy switch
            {
                "DateOld" => filteredAppointments
                    .OrderBy(a => a.AppointmentDate)
                    .ThenBy(a => a.StartTime)
                    .ToList(),
                "Status" => filteredAppointments
                    .OrderBy(a => GetStatusOrder(a.Status))
                    .ThenByDescending(a => a.AppointmentDate)
                    .ToList(),
                _ => filteredAppointments
                    .OrderByDescending(a => a.AppointmentDate)
                    .ThenByDescending(a => a.StartTime)
                    .ToList()
            };

            var totalCount = filteredAppointments.Count;
            var pagedAppointments = filteredAppointments
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            return new PaginatedResult<AppointmentDto>
            {
                Items = _mapper.Map<List<AppointmentDto>>(pagedAppointments),
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                StatusCounts = new Dictionary<string, int>
                {
                    { "Pending", filteredAppointments.Count(a => a.Status == "Pending") },
                    { "Confirmed", filteredAppointments.Count(a => a.Status == "Confirmed") },
                    { "Completed", filteredAppointments.Count(a => a.Status == "Completed") },
                    { "Cancelled", filteredAppointments.Count(a => a.Status == "Cancelled") }
                }
            };
        }

        private static int GetStatusOrder(string status) => status switch
        {
            "Pending" => 1,
            "Confirmed" => 2,
            "Completed" => 3,
            "Cancelled" => 4,
            _ => 5
        };
    }
}
