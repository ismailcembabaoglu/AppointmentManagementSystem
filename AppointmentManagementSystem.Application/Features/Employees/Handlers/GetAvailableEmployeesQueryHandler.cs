using AppointmentManagementSystem.Application.DTOs;
using AppointmentManagementSystem.Application.Features.Employees.Queries;
using AppointmentManagementSystem.Domain.Interfaces;
using AutoMapper;
using MediatR;

namespace AppointmentManagementSystem.Application.Features.Employees.Handlers
{
    public class GetAvailableEmployeesQueryHandler : IRequestHandler<GetAvailableEmployeesQuery, List<EmployeeDto>>
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IMapper _mapper;

        public GetAvailableEmployeesQueryHandler(
            IEmployeeRepository employeeRepository,
            IAppointmentRepository appointmentRepository,
            IMapper mapper)
        {
            _employeeRepository = employeeRepository;
            _appointmentRepository = appointmentRepository;
            _mapper = mapper;
        }

        public async Task<List<EmployeeDto>> Handle(GetAvailableEmployeesQuery request, CancellationToken cancellationToken)
        {
            // İşletmedeki tüm aktif çalışanları al
            var allEmployees = await _employeeRepository.GetByBusinessAsync(request.BusinessId);

            // Seçilen tarih ve zaman diliminde randevusu olan çalışanları bul
            var requestedStartDateTime = request.SelectedDate.Date.Add(request.SelectedTime);
            var requestedEndDateTime = requestedStartDateTime.AddMinutes(request.TotalDurationMinutes);

            // İşletmenin o tarihteki tüm randevularını al
            var businessAppointments = await _appointmentRepository.GetByBusinessAsync(request.BusinessId);
            
            // Sadece seçilen tarihteki ve iptal edilmemiş randevuları filtrele
            var appointmentsOnDate = businessAppointments
                .Where(a => a.AppointmentDate.Date == request.SelectedDate.Date 
                    && a.Status != "Cancelled" 
                    && a.EmployeeId.HasValue)
                .ToList();

            // Zaman çakışması olan çalışanların ID'lerini bul
            var busyEmployeeIds = new HashSet<int>();
            
            foreach (var appointment in appointmentsOnDate)
            {
                var appointmentStartDateTime = appointment.AppointmentDate.Date.Add(appointment.StartTime);
                var appointmentEndDateTime = appointment.AppointmentDate.Date.Add(appointment.EndTime);

                // Zaman çakışması kontrolü:
                // Randevu başlangıcı, istenen zaman diliminin içinde mi?
                // VEYA Randevu bitişi, istenen zaman diliminin içinde mi?
                // VEYA Randevu, istenen zaman dilimini tamamen kapsıyor mu?
                bool hasOverlap = (appointmentStartDateTime < requestedEndDateTime) && 
                                  (appointmentEndDateTime > requestedStartDateTime);

                if (hasOverlap && appointment.EmployeeId.HasValue)
                {
                    busyEmployeeIds.Add(appointment.EmployeeId.Value);
                }
            }

            // Müsait çalışanları filtrele (randevusu olmayan çalışanlar)
            var availableEmployees = allEmployees
                .Where(e => !busyEmployeeIds.Contains(e.Id))
                .ToList();

            return _mapper.Map<List<EmployeeDto>>(availableEmployees);
        }
    }
}
