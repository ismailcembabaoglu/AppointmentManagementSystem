using System.Threading;
using System.Threading.Tasks;
using AppointmentManagementSystem.Application.DTOs.Ai;
using AppointmentManagementSystem.Application.Features.Ai.Queries;
using AppointmentManagementSystem.Application.Services;
using MediatR;

namespace AppointmentManagementSystem.Application.Features.Ai.Handlers
{
    public class GetBusinessAnalyticsQueryHandler : IRequestHandler<GetBusinessAnalyticsQuery, AiBusinessAnalyticsDto?>
    {
        private readonly IBusinessAnalyticsBuilder _analyticsBuilder;

        public GetBusinessAnalyticsQueryHandler(IBusinessAnalyticsBuilder analyticsBuilder)
        {
            _analyticsBuilder = analyticsBuilder;
        }

        public async Task<AiBusinessAnalyticsDto?> Handle(GetBusinessAnalyticsQuery request, CancellationToken cancellationToken)
        {
            return await _analyticsBuilder.BuildAsync(request.BusinessId, cancellationToken);
        }
    }
}
