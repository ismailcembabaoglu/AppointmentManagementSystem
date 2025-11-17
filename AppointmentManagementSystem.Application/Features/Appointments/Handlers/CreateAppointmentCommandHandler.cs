using AppointmentManagementSystem.Application.DTOs;
using AppointmentManagementSystem.Application.Features.Appointments.Commands;
using AppointmentManagementSystem.Domain.Entities;
using AppointmentManagementSystem.Domain.Interfaces;
using AutoMapper;
using MediatR;

namespace AppointmentManagementSystem.Application.Features.Appointments.Handlers
{
    public class CreateAppointmentCommandHandler : IRequestHandler<CreateAppointmentCommand, AppointmentDto>
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IServiceRepository _serviceRepository;
        private readonly IRepository<AppointmentPhoto> _appointmentPhotoRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Business> _businessRepository;
        private readonly IRepository<Employee> _employeeRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;

        public CreateAppointmentCommandHandler(
            IAppointmentRepository appointmentRepository,
            IServiceRepository serviceRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IRepository<AppointmentPhoto> appointmentPhotoRepository,
            IRepository<User> userRepository,
            IRepository<Business> businessRepository,
            IRepository<Employee> employeeRepository,
            IEmailService emailService)
        {
            _appointmentRepository = appointmentRepository;
            _serviceRepository = serviceRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _appointmentPhotoRepository = appointmentPhotoRepository;
            _userRepository = userRepository;
            _businessRepository = businessRepository;
            _employeeRepository = employeeRepository;
            _emailService = emailService;
        }

        public async Task<AppointmentDto> Handle(CreateAppointmentCommand request, CancellationToken cancellationToken)
        {
            // Servis süresini al
            var service = await _serviceRepository.GetByIdAsync(request.CreateAppointmentDto.ServiceId);
            if (service == null)
                throw new Exception("Hizmet bulunamadı.");

            var appointment = _mapper.Map<Appointment>(request.CreateAppointmentDto);
            appointment.Status = "Pending";
            appointment.EndTime = appointment.StartTime.Add(TimeSpan.FromMinutes(service.DurationMinutes));

            await _appointmentRepository.AddAsync(appointment);
            await _unitOfWork.SaveChangesAsync();
            if (request.CreateAppointmentDto.PhotosBase64!=null && request.CreateAppointmentDto.PhotosBase64.Count>0)
            {
                foreach (var photos in request.CreateAppointmentDto.PhotosBase64)
                {
                    if (!string.IsNullOrEmpty(photos))
                    {
                        var appointmentPhoto = new AppointmentPhoto
                        {
                            AppointmentId = appointment.Id,
                            Appointment = appointment,
                            FileName = $"business_{appointment.Id}_photo.jpg",
                            Base64Data = photos,
                            ContentType = "image/jpeg",
                            FileSize = photos.Length
                        };
                        await _appointmentPhotoRepository.AddAsync(appointmentPhoto);
                        await _unitOfWork.SaveChangesAsync();
                    }
                }
            }
           
            var appointmentWithDetails = await _appointmentRepository.GetByIdWithDetailsAsync(appointment.Id);

            // Email gönder
            try
            {
                var customer = await _userRepository.GetByIdAsync(appointment.CustomerId);
                var business = await _businessRepository.GetByIdAsync(appointment.BusinessId);
                var employee = appointment.EmployeeId.HasValue 
                    ? await _employeeRepository.GetByIdAsync(appointment.EmployeeId.Value) 
                    : null;

                if (customer != null && business != null && service != null)
                {
                    await _emailService.SendAppointmentConfirmationAsync(
                        customer.Email,
                        customer.Name,
                        business.Name,
                        service.Name,
                        appointment.AppointmentDate,
                        appointment.StartTime,
                        appointment.EndTime,
                        employee?.Name,
                        appointment.Notes
                    );
                }
            }
            catch (Exception ex)
            {
                // Email gönderilemezse log'la ama randevu oluşturma devam etsin
                Console.WriteLine($"Randevu email'i gönderilemedi: {ex.Message}");
            }

            return _mapper.Map<AppointmentDto>(appointmentWithDetails);
        }
    }
}
