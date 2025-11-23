using AppointmentManagementSystem.Application.DTOs.Ai;
using MediatR;

namespace AppointmentManagementSystem.Application.Features.Ai.Queries
{
    public class GetBusinessReportFileQuery : IRequest<AiBusinessReportFileDto?>
    {
        public int BusinessId { get; set; }
    }
}
