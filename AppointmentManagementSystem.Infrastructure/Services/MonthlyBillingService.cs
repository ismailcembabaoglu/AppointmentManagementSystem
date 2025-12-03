using AppointmentManagementSystem.Domain.Entities;
using AppointmentManagementSystem.Domain.Interfaces;
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
        private Timer? _retryTimer;

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

            // Run payment retry check every 6 hours
            _retryTimer = new Timer(ProcessFailedPayments, null, TimeSpan.FromMinutes(5), TimeSpan.FromHours(6));

            return Task.CompletedTask;
        }

        private async void ProcessDueSubscriptions(object? state)
        {
            _logger.LogInformation("Processing subscriptions due for billing...");

            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppointmentDbContext>();
            var paytrService = scope.ServiceProvider.GetRequiredService<IPayTRService>();

            try
            {
                var today = DateTime.Now.Date;
                var dueSubscriptions = await context.Set<BusinessSubscription>()
                    .Include(s => s.Business)
                    .Where(s => s.IsActive &&
                               s.SubscriptionStatus == SubscriptionStatus.Active &&
                               s.NextBillingDate.HasValue &&
                               s.NextBillingDate.Value.Date <= today)
                    .ToListAsync();

                _logger.LogInformation($"Found {dueSubscriptions.Count} subscriptions due for billing.");

                foreach (var subscription in dueSubscriptions)
                {
                    try
                    {
                        await ChargeSubscriptionAsync(subscription, context, paytrService);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Error charging subscription for Business {subscription.BusinessId}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ProcessDueSubscriptions");
            }
        }

        private async void ProcessFailedPayments(object? state)
        {
            _logger.LogInformation("Processing failed payments for retry...");

            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppointmentDbContext>();
            var paytrService = scope.ServiceProvider.GetRequiredService<IPayTRService>();

            try
            {
                var now = DateTime.Now;
                var failedPayments = await context.Set<Payment>()
                    .Include(p => p.Business)
                        .ThenInclude(b => b!.Subscription)
                    .Where(p => p.Status == PaymentStatus.Failed &&
                               p.RetryCount < p.MaxRetries &&
                               p.NextRetryDate.HasValue &&
                               p.NextRetryDate.Value <= now)
                    .ToListAsync();

                _logger.LogInformation($"Found {failedPayments.Count} failed payments to retry.");

                foreach (var payment in failedPayments)
                {
                    try
                    {
                        await RetryPaymentAsync(payment, context, paytrService);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Error retrying payment {payment.Id}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ProcessFailedPayments");
            }
        }

        private async Task ChargeSubscriptionAsync(
            BusinessSubscription subscription,
            AppointmentDbContext context,
            IPayTRService paytrService)
        {
            var merchantOid = Guid.NewGuid().ToString();

            // Create payment record
            var payment = new Payment
            {
                BusinessId = subscription.BusinessId,
                MerchantOid = merchantOid,
                Amount = subscription.MonthlyAmount,
                Currency = subscription.Currency,
                Status = PaymentStatus.Pending,
                CreatedAt = DateTime.Now
            };

            context.Set<Payment>().Add(payment);
            await context.SaveChangesAsync();

            // Attempt to charge via PayTR
            var result = await paytrService.ChargeRecurringPaymentAsync(
                customerEmail: subscription.Business?.Email ?? "business@example.com",
                utoken: subscription.PayTRUserToken ?? "",
                ctoken: subscription.PayTRCardToken ?? "",
                merchantOid: merchantOid,
                amount: subscription.MonthlyAmount,
                userIp: "127.0.0.1" // Server IP for automated payments
            );

            payment.PayTRResponse = result.RawResponse;
            payment.PayTRTransactionId = result.TransactionId;

            if (result.Success && result.Status == "success")
            {
                payment.Status = PaymentStatus.Success;
                payment.PaymentDate = DateTime.Now;

                // Update subscription
                subscription.LastBillingDate = DateTime.Now;
                subscription.NextBillingDate = DateTime.Now.AddDays(30);
                subscription.SubscriptionStatus = SubscriptionStatus.Active;

                // Ensure business is active
                if (subscription.Business != null)
                {
                    subscription.Business.IsActive = true;
                }

                _logger.LogInformation($"Payment {payment.Id} charged successfully for Business {subscription.BusinessId}");
            }
            else
            {
                payment.Status = PaymentStatus.Failed;
                payment.ErrorMessage = result.ErrorMessage ?? "Payment declined";
                payment.RetryCount = 0;
                payment.NextRetryDate = DateTime.Now.AddHours(1); // Retry after 1 hour

                // Suspend business on payment failure
                subscription.SubscriptionStatus = SubscriptionStatus.Suspended;
                if (subscription.Business != null)
                {
                    subscription.Business.IsActive = false;
                }

                _logger.LogWarning($"Payment {payment.Id} failed for Business {subscription.BusinessId}: {payment.ErrorMessage}");
            }

            await context.SaveChangesAsync();
        }

        private async Task RetryPaymentAsync(
            Payment payment,
            AppointmentDbContext context,
            IPayTRService paytrService)
        {
            var subscription = await context.Set<BusinessSubscription>()
                .Include(s => s.Business)
                .FirstOrDefaultAsync(s => s.BusinessId == payment.BusinessId);

            if (subscription == null || string.IsNullOrEmpty(subscription.PayTRUserToken) || string.IsNullOrEmpty(subscription.PayTRCardToken))
            {
                _logger.LogWarning($"Cannot retry payment {payment.Id}: Missing subscription or tokens");
                return;
            }

            payment.RetryCount++;

            var result = await paytrService.ChargeRecurringPaymentAsync(
                customerEmail: subscription.Business?.Email ?? "business@example.com",
                utoken: subscription.PayTRUserToken,
                ctoken: subscription.PayTRCardToken,
                merchantOid: payment.MerchantOid,
                amount: payment.Amount,
                userIp: "127.0.0.1"
            );

            payment.PayTRResponse = result.RawResponse;
            payment.UpdatedAt = DateTime.Now;

            if (result.Success && result.Status == "success")
            {
                payment.Status = PaymentStatus.Success;
                payment.PaymentDate = DateTime.Now;
                payment.PayTRTransactionId = result.TransactionId;

                // Reactivate subscription and business
                subscription.SubscriptionStatus = SubscriptionStatus.Active;
                subscription.LastBillingDate = DateTime.Now;
                subscription.NextBillingDate = DateTime.Now.AddDays(30);

                if (subscription.Business != null)
                {
                    subscription.Business.IsActive = true;
                }

                _logger.LogInformation($"Payment {payment.Id} succeeded on retry {payment.RetryCount}");
            }
            else
            {
                // Schedule next retry with exponential backoff
                var retryHours = Math.Min(24 * (int)Math.Pow(2, payment.RetryCount - 1), 336); // Max 14 days
                payment.NextRetryDate = DateTime.Now.AddHours(retryHours);
                payment.ErrorMessage = result.ErrorMessage;

                _logger.LogWarning($"Payment {payment.Id} retry {payment.RetryCount} failed. Next retry in {retryHours} hours");
            }

            await context.SaveChangesAsync();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Monthly Billing Service is stopping.");

            _billingTimer?.Change(Timeout.Infinite, 0);
            _retryTimer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _billingTimer?.Dispose();
            _retryTimer?.Dispose();
        }
    }
}
