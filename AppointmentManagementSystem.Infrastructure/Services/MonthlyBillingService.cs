using AppointmentManagementSystem.Domain.Entities;
using AppointmentManagementSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AppointmentManagementSystem.Infrastructure.Services
{
    public class MonthlyBillingService : IHostedService, IDisposable
    {
        private readonly ILogger<MonthlyBillingService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private Timer? _billingTimer;

        public MonthlyBillingService(ILogger<MonthlyBillingService> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Monthly Billing Service is starting.");

            // Run billing check daily at 02:00 AM
            _billingTimer = new Timer(ProcessDueSubscriptions, null, TimeSpan.Zero, TimeSpan.FromHours(24));

            return Task.CompletedTask;
        }

        private async void ProcessDueSubscriptions(object? state)
        {
            _logger.LogInformation("Checking subscriptions for overdue billing dates...");

            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppointmentDbContext>();

            try
            {
                var today = DateTime.Now.Date;
                var dueSubscriptions = await context.Set<BusinessSubscription>()
                    .Include(s => s.Business)
                    .Where(s => s.IsActive &&
                               s.SubscriptionStatus == SubscriptionStatus.Active &&
                               s.NextBillingDate.HasValue &&
                               today > s.NextBillingDate.Value.Date)
                    .ToListAsync();

                _logger.LogInformation($"Found {dueSubscriptions.Count} subscriptions past their billing date.");

                foreach (var subscription in dueSubscriptions)
                {
                    if (subscription.Business != null)
                    {
                        subscription.Business.IsActive = false;
                        _logger.LogWarning($"Business {subscription.BusinessId} deactivated due to overdue billing date ({subscription.NextBillingDate:yyyy-MM-dd}).");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ProcessDueSubscriptions");
            }

            await context.SaveChangesAsync();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Monthly Billing Service is stopping.");

            _billingTimer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _billingTimer?.Dispose();
        }
    }
}
