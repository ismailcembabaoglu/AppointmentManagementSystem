using MediatR;

namespace AppointmentManagementSystem.Application.Features.Documents.Commands
{
    public class DeleteDocumentCommand : IRequest<bool>
    {
        public int DocumentId { get; set; }
    }
}
