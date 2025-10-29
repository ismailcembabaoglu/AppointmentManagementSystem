using AppointmentManagementSystem.Application.Features.Documents.Commands;
using AppointmentManagementSystem.Domain.Interfaces;
using MediatR;

namespace AppointmentManagementSystem.Application.Features.Documents.Handlers
{
    public class DeleteDocumentCommandHandler : IRequestHandler<DeleteDocumentCommand, bool>
    {
        private readonly IEmployeeDocumentRepository _documentRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteDocumentCommandHandler(
            IEmployeeDocumentRepository documentRepository,
            IUnitOfWork unitOfWork)
        {
            _documentRepository = documentRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(DeleteDocumentCommand request, CancellationToken cancellationToken)
        {
            var document = await _documentRepository.GetByIdAsync(request.DocumentId);
            if (document == null)
                return false;

            // Soft delete
            document.IsDeleted = true;
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
