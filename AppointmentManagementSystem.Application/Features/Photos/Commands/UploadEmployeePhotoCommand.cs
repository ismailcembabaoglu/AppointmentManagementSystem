using AppointmentManagementSystem.Application.DTOs;
using MediatR;

namespace AppointmentManagementSystem.Application.Features.Photos.Commands
{
    public class UploadEmployeePhotoCommand : IRequest<PhotoDto>
    {
        public int EmployeeId { get; set; }
        public UploadPhotoDto PhotoDto { get; set; } = new();
    }
}
