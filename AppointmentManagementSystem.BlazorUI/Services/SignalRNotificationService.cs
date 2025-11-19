using AppointmentManagementSystem.Application.DTOs;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.SignalR.Client;

namespace AppointmentManagementSystem.BlazorUI.Services
{
    public class SignalRNotificationService : IAsyncDisposable
    {
        private readonly IConfiguration _configuration;
        private readonly ILocalStorageService _localStorage;
        private HubConnection? _hubConnection;
        
        // Events for UI components to subscribe to
        public event Action<AppointmentDto>? OnAppointmentCreated;
        public event Action<AppointmentDto>? OnAppointmentStatusChanged;
        public event Action<int>? OnAppointmentDeleted;
        public event Action? OnConnectionStateChanged;

        public bool IsConnected => _hubConnection?.State == HubConnectionState.Connected;

        public SignalRNotificationService(IConfiguration configuration, ILocalStorageService localStorage)
        {
            _configuration = configuration;
            _localStorage = localStorage;
        }

        public async Task StartAsync()
        {
            if (_hubConnection != null)
            {
                await StopAsync();
            }

            try
            {
                string? token = null;
                try
                {
                    token = await _localStorage.GetItemAsStringAsync("authToken");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"SignalR: Error getting token from localStorage: {ex.Message}");
                }

                if (string.IsNullOrEmpty(token))
                {
                    Console.WriteLine("SignalR: No auth token found. Cannot start connection.");
                    return;
                }

                // Token'dan tırnak işaretlerini temizle
                token = token.Replace("\"", "").Trim();

                var apiBaseUrl = _configuration["ApiBaseUrl"] ?? "https://hub.aptivaplan.com.tr";
                var hubUrl = $"{apiBaseUrl}/notificationHub";
                
                Console.WriteLine($"SignalR: Attempting to connect to {hubUrl}");

                _hubConnection = new HubConnectionBuilder()
                    .WithUrl(hubUrl, options =>
                    {
                        options.AccessTokenProvider = () => Task.FromResult<string?>(token);
                        options.Transports = Microsoft.AspNetCore.Http.Connections.HttpTransportType.WebSockets | 
                                            Microsoft.AspNetCore.Http.Connections.HttpTransportType.LongPolling;
                    })
                    .WithAutomaticReconnect(new[] { TimeSpan.Zero, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(10) })
                    .ConfigureLogging(logging =>
                    {
                        logging.SetMinimumLevel(LogLevel.Information);
                    })
                    .Build();

                // Event handlers
                _hubConnection.On<AppointmentDto>("AppointmentCreated", (appointment) =>
                {
                    Console.WriteLine($"SignalR: Appointment created - ID: {appointment.Id}");
                    OnAppointmentCreated?.Invoke(appointment);
                });

                _hubConnection.On<AppointmentDto>("AppointmentStatusChanged", (appointment) =>
                {
                    Console.WriteLine($"SignalR: Appointment status changed - ID: {appointment.Id}, Status: {appointment.Status}");
                    OnAppointmentStatusChanged?.Invoke(appointment);
                });

                _hubConnection.On<int>("AppointmentDeleted", (appointmentId) =>
                {
                    Console.WriteLine($"SignalR: Appointment deleted - ID: {appointmentId}");
                    OnAppointmentDeleted?.Invoke(appointmentId);
                });

                _hubConnection.Reconnecting += error =>
                {
                    Console.WriteLine($"SignalR: Reconnecting... Error: {error?.Message}");
                    OnConnectionStateChanged?.Invoke();
                    return Task.CompletedTask;
                };

                _hubConnection.Reconnected += connectionId =>
                {
                    Console.WriteLine($"SignalR: Reconnected with ID: {connectionId}");
                    OnConnectionStateChanged?.Invoke();
                    return Task.CompletedTask;
                };

                _hubConnection.Closed += error =>
                {
                    Console.WriteLine($"SignalR: Connection closed. Error: {error?.Message}");
                    OnConnectionStateChanged?.Invoke();
                    return Task.CompletedTask;
                };

                await _hubConnection.StartAsync();
                Console.WriteLine($"SignalR: Connected successfully to {hubUrl}");
                OnConnectionStateChanged?.Invoke();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SignalR: Error starting connection: {ex.Message}");
            }
        }

        public async Task StopAsync()
        {
            if (_hubConnection != null)
            {
                try
                {
                    await _hubConnection.StopAsync();
                    await _hubConnection.DisposeAsync();
                    _hubConnection = null;
                    Console.WriteLine("SignalR: Connection stopped");
                    OnConnectionStateChanged?.Invoke();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"SignalR: Error stopping connection: {ex.Message}");
                }
            }
        }

        public async ValueTask DisposeAsync()
        {
            await StopAsync();
        }
    }
}
