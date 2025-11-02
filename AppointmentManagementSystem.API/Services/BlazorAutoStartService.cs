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
                Task.Delay(3000, cancellationToken).Wait(cancellationToken);

                var currentDirectory = Directory.GetCurrentDirectory();
                var blazorProjectPath = Path.Combine(currentDirectory, "..", "AppointmentManagementSystem.BlazorUI");
                blazorProjectPath = Path.GetFullPath(blazorProjectPath);

                if (!Directory.Exists(blazorProjectPath))
                {
                    _logger.LogWarning("⚠️  Blazor project not found at: {Path}", blazorProjectPath);
                    return;
                }

                _logger.LogInformation("🚀 Starting Blazor UI automatically...");
                _logger.LogInformation("📁 Blazor Path: {Path}", blazorProjectPath);

                var processStartInfo = new ProcessStartInfo
                {
                    FileName = "dotnet",
                    Arguments = "run",
                    WorkingDirectory = blazorProjectPath,
                    UseShellExecute = true,
                    CreateNoWindow = false,
                    WindowStyle = ProcessWindowStyle.Normal
                };

                // Environment variable to disable Visual Studio tooling
                processStartInfo.Environment["ASPNETCORE_HOSTINGSTARTUPASSEMBLIES"] = "";

                _blazorProcess = Process.Start(processStartInfo);

                if (_blazorProcess != null)
                {
                    _logger.LogInformation("✅ Blazor UI started successfully!");
                    _logger.LogInformation("🌐 Blazor will be available at: http://localhost:5090");
                    _logger.LogInformation("⏳ Wait for Blazor to compile and start (may take 10-30 seconds)");
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
                    
                    // Kill the process tree
                    try
                    {
                        _blazorProcess.Kill(entireProcessTree: true);
                    }
                    catch
                    {
                        // If Kill with tree fails, try simple kill
                        _blazorProcess.Kill();
                    }
                    
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
