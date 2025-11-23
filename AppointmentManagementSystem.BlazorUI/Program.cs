using AppointmentManagementSystem.BlazorUI;
using AppointmentManagementSystem.BlazorUI.Services;
using AppointmentManagementSystem.BlazorUI.Services.ApiServices;
using AppointmentManagementSystem.BlazorUI.Services.Authentication;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Radzen;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configuration for API Base URL
builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
{
    { "ApiBaseUrl", "https://hub.aptivaplan.com.tr" }
});

// Local Storage (diğer servislerden önce ekle)
builder.Services.AddBlazoredLocalStorage();

// HttpClient yapılandırması - Basit ve çalışır
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("https://hub.aptivaplan.com.tr/"),
});
//builder.Services.AddScoped(sp => new HttpClient
//{
//    BaseAddress = new Uri("https://localhost:5089/"),
//});

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
builder.Services.AddScoped<IAdminApiService, AdminApiService>();

// SignalR Notification Service (Scoped olarak değiştirdik)
builder.Services.AddScoped<SignalRNotificationService>();

// Authentication
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();

// Radzen servislerini ekle
builder.Services.AddScoped<DialogService>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<TooltipService>();
builder.Services.AddScoped<ContextMenuService>();
await builder.Build().RunAsync();
