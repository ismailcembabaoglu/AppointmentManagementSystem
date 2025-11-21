using AppointmentManagementSystem.Application.Features.Appointments.Queries;
using AppointmentManagementSystem.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentManagementSystem.Application.Features.Appointments.Handlers
{
    public class GetAppointmentPhotosQueryHandler : IRequestHandler<GetAppointmentPhotosQuery, List<string>>
    {
        private readonly IAppointmentRepository _appointmentRepository;

        public GetAppointmentPhotosQueryHandler(IAppointmentRepository appointmentRepository)
        {
            _appointmentRepository = appointmentRepository;
        }

        public async Task<List<string>> Handle(GetAppointmentPhotosQuery request, CancellationToken cancellationToken)
        {
            var photos = await _appointmentRepository.GetAppointmentPhotosAsync(request.AppointmentId);

            // Base64 string'lere dönüştür
            return photos
                .Select(p => p.Base64Data?? string.Empty)
                .ToList();
        }
    }
}
