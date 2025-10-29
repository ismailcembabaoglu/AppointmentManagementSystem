using AppointmentManagementSystem.Application.DTOs;
using MediatR;

namespace AppointmentManagementSystem.Application.Features.Documents.Commands
{
    public class UploadEmployeeDocumentCommand : IRequest<DocumentDto>
    {
        public int EmployeeId { get; set; }
        public UploadDocumentDto DocumentDto { get; set; } = new();
    }
}
