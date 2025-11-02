using AppointmentManagementSystem.Application.DTOs;
using AppointmentManagementSystem.Application.Features.Employees.Queries;
using AppointmentManagementSystem.Domain.Interfaces;
using AutoMapper;
using MediatR;

namespace AppointmentManagementSystem.Application.Features.Employees.Handlers
{
    public class GetAvailableEmployeesQueryHandler : IRequestHandler<GetAvailableEmployeesQuery, AvailableEmployeesResponseDto>
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

        public async Task<AvailableEmployeesResponseDto> Handle(GetAvailableEmployeesQuery request, CancellationToken cancellationToken)
        {
            var response = new AvailableEmployeesResponseDto();

            // İşletmedeki tüm aktif çalışanları al
            var allEmployees = (await _employeeRepository.GetByBusinessAsync(request.BusinessId)).ToList();

            if (!allEmployees.Any())
            {
                return response; // Hiç çalışan yok
            }

            // İşletmenin tüm randevularını al
            var businessAppointments = (await _appointmentRepository.GetByBusinessAsync(request.BusinessId))
                .Where(a => a.Status != "Cancelled" && a.EmployeeId.HasValue)
                .ToList();

            // Seçilen tarih ve saatte müsait çalışanları bul
            var availableEmployees = GetAvailableEmployeesForTimeSlot(
                allEmployees,
                businessAppointments,
                request.SelectedDate,
                request.SelectedTime,
                request.TotalDurationMinutes
            );

            if (availableEmployees.Any())
            {
                // Müsait çalışan var
                response.Employees = _mapper.Map<List<EmployeeDto>>(availableEmployees);
            }
            else
            {
                // Müsait çalışan yok - en erken müsait zamanı bul
                var nextAvailable = FindNextAvailableSlot(
                    allEmployees,
                    businessAppointments,
                    request.SelectedDate,
                    request.SelectedTime,
                    request.TotalDurationMinutes
                );

                if (nextAvailable.HasValue)
                {
                    response.NextAvailableDateTime = nextAvailable.Value;
                    
                    var timeSpan = nextAvailable.Value - request.SelectedDate.Date.Add(request.SelectedTime);
                    
                    if (timeSpan.TotalHours < 1)
                    {
                        response.NextAvailableMessage = $"En erken {(int)timeSpan.TotalMinutes} dakika sonra müsait çalışan bulunmaktadır.";
                    }
                    else if (timeSpan.TotalHours < 24)
                    {
                        response.NextAvailableMessage = $"En erken {(int)timeSpan.TotalHours} saat sonra müsait çalışan bulunmaktadır.";
                    }
                    else
                    {
                        var daysDiff = (int)timeSpan.TotalDays;
                        var formattedDate = nextAvailable.Value.ToString("dd MMMM yyyy, HH:mm");
                        response.NextAvailableMessage = $"En erken {daysDiff} gün sonra ({formattedDate}) müsait çalışan bulunmaktadır.";
                    }
                }
                else
                {
                    response.NextAvailableMessage = "Önümüzdeki 7 gün içinde müsait çalışan bulunamamaktadır.";
                }
            }

            return response;
        }

        private List<Domain.Entities.Employee> GetAvailableEmployeesForTimeSlot(
            List<Domain.Entities.Employee> allEmployees,
            List<Domain.Entities.Appointment> appointments,
            DateTime selectedDate,
            TimeSpan selectedTime,
            int durationMinutes)
        {
            var requestedStartDateTime = selectedDate.Date.Add(selectedTime);
            var requestedEndDateTime = requestedStartDateTime.AddMinutes(durationMinutes);

            // O tarihteki randevuları filtrele
            var appointmentsOnDate = appointments
                .Where(a => a.AppointmentDate.Date == selectedDate.Date)
                .ToList();

            // Zaman çakışması olan çalışanların ID'lerini bul
            var busyEmployeeIds = new HashSet<int>();

            foreach (var appointment in appointmentsOnDate)
            {
                var appointmentStartDateTime = appointment.AppointmentDate.Date.Add(appointment.StartTime);
                var appointmentEndDateTime = appointment.AppointmentDate.Date.Add(appointment.EndTime);

                bool hasOverlap = (appointmentStartDateTime < requestedEndDateTime) &&
                                  (appointmentEndDateTime > requestedStartDateTime);

                if (hasOverlap && appointment.EmployeeId.HasValue)
                {
                    busyEmployeeIds.Add(appointment.EmployeeId.Value);
                }
            }

            // Müsait çalışanları döndür
            return allEmployees.Where(e => !busyEmployeeIds.Contains(e.Id)).ToList();
        }

        private DateTime? FindNextAvailableSlot(
            List<Domain.Entities.Employee> allEmployees,
            List<Domain.Entities.Appointment> appointments,
            DateTime selectedDate,
            TimeSpan selectedTime,
            int durationMinutes)
        {
            // Çalışma saatleri: 09:00 - 18:00, her 30 dakikada bir
            var workStartHour = 9;
            var workEndHour = 18;
            var slotIntervalMinutes = 30;

            // Önümüzdeki 7 günü kontrol et
            for (int day = 0; day < 7; day++)
            {
                var checkDate = selectedDate.AddDays(day);

                // Eğer aynı gün ise, seçilen saatten sonraki slotları kontrol et
                var startHour = (day == 0) ? Math.Max(workStartHour, selectedTime.Hours) : workStartHour;
                var startMinute = (day == 0 && selectedTime.Hours >= workStartHour) 
                    ? ((selectedTime.Minutes / slotIntervalMinutes) + 1) * slotIntervalMinutes 
                    : 0;

                // O günün tüm zaman dilimlerini kontrol et
                for (int hour = startHour; hour < workEndHour; hour++)
                {
                    for (int minute = (hour == startHour ? startMinute : 0); minute < 60; minute += slotIntervalMinutes)
                    {
                        var checkTime = new TimeSpan(hour, minute, 0);
                        
                        // Bu zaman diliminde en az bir müsait çalışan var mı?
                        var availableEmployees = GetAvailableEmployeesForTimeSlot(
                            allEmployees,
                            appointments,
                            checkDate,
                            checkTime,
                            durationMinutes
                        );

                        if (availableEmployees.Any())
                        {
                            return checkDate.Date.Add(checkTime);
                        }
                    }
                }
            }

            return null; // 7 gün içinde müsait slot bulunamadı
        }
    }
}
