using AppointmentManagementSystem.Application.DTOs;
using AppointmentManagementSystem.Application.Features.Documents.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentManagementSystem.API.Controllers
{
    [Authorize]
    public class DocumentsController : BaseController
    {
        private readonly IMediator _mediator;

        public DocumentsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Upload document for an employee
        /// </summary>
        [HttpPost("employee/{employeeId:int}")]
        [Authorize(Roles = "Business,Admin")]
        public async Task<IActionResult> UploadEmployeeDocument(int employeeId, [FromBody] UploadDocumentDto documentDto)
        {
            try
            {
                var command = new UploadEmployeeDocumentCommand
                {
                    EmployeeId = employeeId,
                    DocumentDto = documentDto
                };
                var result = await _mediator.Send(command);
                return OkResponse(result, "Doküman başarıyla yüklendi.");
            }
            catch (Exception ex)
            {
                return ErrorResponse<DocumentDto>(ex.Message, new List<string> { ex.Message });
            }
        }

        /// <summary>
        /// Delete a document by ID
        /// </summary>
        [HttpDelete("{documentId:int}")]
        [Authorize(Roles = "Business,Admin")]
        public async Task<IActionResult> DeleteDocument(int documentId)
        {
            try
            {
                var command = new DeleteDocumentCommand { DocumentId = documentId };
                var result = await _mediator.Send(command);

                if (!result)
                    return ErrorResponse<bool>("Doküman bulunamadı.", new List<string> { "Geçersiz doküman ID" });

                return OkResponse(result, "Doküman başarıyla silindi.");
            }
            catch (Exception ex)
            {
                return ErrorResponse<bool>(ex.Message, new List<string> { ex.Message });
            }
        }
    }
}
