using System.Diagnostics;

namespace AppointmentManagementSystem.API.Services
{
    public class BlazorAutoStartService : IHostedService
    {
        private readonly ILogger<BlazorAutoStartService> _logger;
        private readonly IHostEnvironment _environment;
        private readonly IConfiguration _configuration;
        private Process? _blazorProcess;

        public BlazorAutoStartService(
            ILogger<BlazorAutoStartService> logger,
            IHostEnvironment environment,
            IConfiguration configuration)
        {
            _logger = logger;
            _environment = environment;
            _configuration = configuration;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            // Only auto-start in development
            var autoStart = _configuration.GetValue<bool>("BlazorUI:AutoStart", true);
            
            if (_environment.IsDevelopment() && autoStart)
            {
                Task.Run(() => StartBlazorUI(cancellationToken), cancellationToken);
            }

            return Task.CompletedTask;
        }

        private void StartBlazorUI(CancellationToken cancellationToken)
        {
            try
            {
                // Wait a bit for API to start
                Task.Delay(2000, cancellationToken).Wait(cancellationToken);

                var currentDirectory = Directory.GetCurrentDirectory();
                var blazorProjectPath = Path.Combine(currentDirectory, "..", "AppointmentManagementSystem.BlazorUI");

                if (!Directory.Exists(blazorProjectPath))
                {
                    _logger.LogWarning("⚠️  Blazor project not found at: {Path}", blazorProjectPath);
                    return;
                }

                _logger.LogInformation("🚀 Starting Blazor UI automatically...");

                var processStartInfo = new ProcessStartInfo
                {
                    FileName = "dotnet",
                    Arguments = "run --no-build",
                    WorkingDirectory = blazorProjectPath,
                    UseShellExecute = true,
                    CreateNoWindow = false
                };

                _blazorProcess = Process.Start(processStartInfo);

                if (_blazorProcess != null)
                {
                    _logger.LogInformation("✅ Blazor UI started successfully!");
                    _logger.LogInformation("📁 Blazor Path: {Path}", blazorProjectPath);
                    _logger.LogInformation("🌐 Blazor should be available at: https://localhost:5002 or check the Blazor console");
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Blazor auto-start cancelled");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error starting Blazor UI: {Message}", ex.Message);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            try
            {
                if (_blazorProcess != null && !_blazorProcess.HasExited)
                {
                    _logger.LogInformation("🛑 Stopping Blazor UI...");
                    _blazorProcess.Kill(true); // Kill entire process tree
                    _blazorProcess.Dispose();
                    _logger.LogInformation("✅ Blazor UI stopped");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error stopping Blazor UI: {Message}", ex.Message);
            }

            return Task.CompletedTask;
        }
    }
}
