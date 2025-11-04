using Microsoft.Extensions.Logging;
using AppointmentManagementSystem.Maui.Services;
using AppointmentManagementSystem.Maui.Services.ApiServices;
using AppointmentManagementSystem.Maui.Services.Authentication;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Radzen;
using Plugin.Permissions.Abstractions;

namespace AppointmentManagementSystem.Maui
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMauiBlazorWebView();

            // Configure HttpClient for API
            builder.Services.AddScoped(sp => new HttpClient 
            { 
                BaseAddress = new Uri("http://192.168.1.201:5089/") 
            });

            // API Services
            builder.Services.AddScoped<IApiService, ApiService>();
            builder.Services.AddScoped<ICategoryApiService, CategoryApiService>();
            builder.Services.AddScoped<IBusinessApiService, BusinessApiService>();
            builder.Services.AddScoped<IServiceApiService, ServiceApiService>();
            builder.Services.AddScoped<IEmployeeApiService, EmployeeApiService>();
            builder.Services.AddScoped<IAppointmentApiService, AppointmentApiService>();
            builder.Services.AddScoped<IPhotoApiService, PhotoApiService>();
            builder.Services.AddScoped<IDocumentApiService, DocumentApiService>();
            builder.Services.AddScoped<IPaymentApiService, PaymentApiService>();

            // Local Storage
            builder.Services.AddBlazoredLocalStorage();

            // Authentication
            builder.Services.AddAuthorizationCore();
            builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();

            // Radzen Services
            builder.Services.AddScoped<DialogService>();
            builder.Services.AddScoped<NotificationService>();
            builder.Services.AddScoped<TooltipService>();
            builder.Services.AddScoped<ContextMenuService>();

            // Native Features Services
            builder.Services.AddSingleton<ICameraService, CameraService>();
            builder.Services.AddSingleton<ILocationService, LocationService>();
            //builder.Services.AddScoped<IPermissions>();

#if DEBUG
    		builder.Services.AddBlazorWebViewDeveloperTools();
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
