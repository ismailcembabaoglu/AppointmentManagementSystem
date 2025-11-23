using System.Threading;
using System.Threading.Tasks;

namespace AppointmentManagementSystem.Domain.Interfaces
{
    public interface IGeminiClient
    {
        Task<string> GenerateContentAsync(string prompt, CancellationToken cancellationToken = default);
    }
}
