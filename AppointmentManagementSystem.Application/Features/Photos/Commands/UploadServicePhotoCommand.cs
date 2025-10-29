using AppointmentManagementSystem.Application.DTOs;
using MediatR;

namespace AppointmentManagementSystem.Application.Features.Photos.Commands
{
    public class UploadServicePhotoCommand : IRequest<PhotoDto>
    {
        public int ServiceId { get; set; }
        public UploadPhotoDto PhotoDto { get; set; } = new();
    }
}
