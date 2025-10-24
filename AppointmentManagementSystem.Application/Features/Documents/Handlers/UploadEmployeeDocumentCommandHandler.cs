using AppointmentManagementSystem.Application.DTOs;
using AppointmentManagementSystem.Application.Features.Documents.Commands;
using AppointmentManagementSystem.Domain.Entities;
using AppointmentManagementSystem.Domain.Interfaces;
using AutoMapper;
using MediatR;

namespace AppointmentManagementSystem.Application.Features.Documents.Handlers
{
    public class UploadEmployeeDocumentCommandHandler : IRequestHandler<UploadEmployeeDocumentCommand, DocumentDto>
    {
        private readonly IEmployeeDocumentRepository _documentRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UploadEmployeeDocumentCommandHandler(
            IEmployeeDocumentRepository documentRepository,
            IEmployeeRepository employeeRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _documentRepository = documentRepository;
            _employeeRepository = employeeRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<DocumentDto> Handle(UploadEmployeeDocumentCommand request, CancellationToken cancellationToken)
        {
            var employee = await _employeeRepository.GetByIdAsync(request.EmployeeId);
            if (employee == null)
                throw new Exception($"Employee with ID {request.EmployeeId} not found.");

            if (string.IsNullOrEmpty(request.DocumentDto.Base64Data))
                throw new Exception("Base64 data is required.");

            var base64Length = request.DocumentDto.Base64Data.Length;
            var fileSize = (long)(base64Length * 0.75);

            // Max 25MB for documents
            if (fileSize > 25 * 1024 * 1024)
                throw new Exception("File size exceeds 25MB limit.");

            var allowedTypes = new[] { "application/pdf", "application/msword",
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document" };
            if (!string.IsNullOrEmpty(request.DocumentDto.ContentType) &&
                !allowedTypes.Contains(request.DocumentDto.ContentType.ToLower()))
                throw new Exception("Invalid file type. Only PDF, DOC, and DOCX are allowed.");

            var document = new EmployeeDocument
            {
                EmployeeId = request.EmployeeId,
                Name = request.DocumentDto.Name,
                Description = request.DocumentDto.Description,
                FileName = request.DocumentDto.FileName,
                ContentType = request.DocumentDto.ContentType,
                FileSize = fileSize,
                Base64Data = request.DocumentDto.Base64Data,
                FilePath = null
            };

            await _documentRepository.AddAsync(document);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<DocumentDto>(document);
        }
    }
}
