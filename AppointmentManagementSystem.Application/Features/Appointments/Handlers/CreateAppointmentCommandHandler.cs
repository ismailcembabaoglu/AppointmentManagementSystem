using AppointmentManagementSystem.Application.DTOs;
using AppointmentManagementSystem.Application.Features.Appointments.Commands;
using AppointmentManagementSystem.Domain.Entities;
using AppointmentManagementSystem.Domain.Interfaces;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace AppointmentManagementSystem.Application.Features.Appointments.Handlers
{
    public class CreateAppointmentCommandHandler : IRequestHandler<CreateAppointmentCommand, AppointmentDto>
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IServiceRepository _serviceRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public CreateAppointmentCommandHandler(
            IAppointmentRepository appointmentRepository,
            IServiceRepository serviceRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IConfiguration configuration)
        {
            _appointmentRepository = appointmentRepository;
            _serviceRepository = serviceRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _configuration = configuration;
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

            // Resimleri kaydet
            if (request.CreateAppointmentDto.Photos != null && request.CreateAppointmentDto.Photos.Any())
            {
                var uploadPath = _configuration["FileUploadSettings:UploadPath"] ?? "uploads/appointments";
                var fullPath = Path.Combine(Directory.GetCurrentDirectory(), uploadPath);

                if (!Directory.Exists(fullPath))
                {
                    Directory.CreateDirectory(fullPath);
                }

                appointment.Photos = new List<AppointmentPhoto>();

                foreach (var photoDto in request.CreateAppointmentDto.Photos)
                {
                    var uniqueFileName = $"{Guid.NewGuid()}_{photoDto.FileName}";
                    var filePath = Path.Combine(fullPath, uniqueFileName);

                    await File.WriteAllBytesAsync(filePath, photoDto.FileData, cancellationToken);

                    var photo = new AppointmentPhoto
                    {
                        FileName = uniqueFileName,
                        FilePath = Path.Combine(uploadPath, uniqueFileName),
                        ContentType = photoDto.ContentType,
                        FileSize = photoDto.FileSize
                    };

                    appointment.Photos.Add(photo);
                }
            }

            await _appointmentRepository.AddAsync(appointment);
            await _unitOfWork.SaveChangesAsync();

            var appointmentWithDetails = await _appointmentRepository.GetByIdWithDetailsAsync(appointment.Id);
            return _mapper.Map<AppointmentDto>(appointmentWithDetails);
        }
    }
}
