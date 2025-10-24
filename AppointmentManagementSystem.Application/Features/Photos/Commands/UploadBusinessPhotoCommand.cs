using AppointmentManagementSystem.Application.DTOs;
using MediatR;

namespace AppointmentManagementSystem.Application.Features.Photos.Commands
{
    public class UploadBusinessPhotoCommand : IRequest<PhotoDto>
    {
        public int BusinessId { get; set; }
        public UploadPhotoDto PhotoDto { get; set; } = new();
    }
}
