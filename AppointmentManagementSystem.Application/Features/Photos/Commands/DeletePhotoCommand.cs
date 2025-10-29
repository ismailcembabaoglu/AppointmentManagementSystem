using MediatR;

namespace AppointmentManagementSystem.Application.Features.Photos.Commands
{
    public class DeletePhotoCommand : IRequest<bool>
    {
        public int PhotoId { get; set; }
    }
}
