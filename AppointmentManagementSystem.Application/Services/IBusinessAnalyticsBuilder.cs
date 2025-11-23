using System.Threading;
using System.Threading.Tasks;
using AppointmentManagementSystem.Application.DTOs.Ai;

namespace AppointmentManagementSystem.Application.Services
{
    public interface IBusinessAnalyticsBuilder
    {
        Task<AiBusinessAnalyticsDto?> BuildAsync(int businessId, CancellationToken cancellationToken = default);
    }
}
