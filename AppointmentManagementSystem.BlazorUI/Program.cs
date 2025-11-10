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

// Local Storage (diğer servislerden önce ekle)
builder.Services.AddBlazoredLocalStorage();

// HttpClient yapılandırması - Blazor WebAssembly için
builder.Services.AddScoped(sp =>
{
    var localStorage = sp.GetRequiredService<ILocalStorageService>();
    var authHandler = new AuthorizationMessageHandler(localStorage);
    
    // Blazor WebAssembly'de InnerHandler'ı manuel set etmemize gerek yok
    // Tarayıcının kendi HTTP stack'i kullanılır
    var httpClient = new HttpClient(authHandler)
    {
        BaseAddress = new Uri("https://localhost:5089/"),
        Timeout = TimeSpan.FromSeconds(30) // 30 saniye timeout
    };

    return httpClient;
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
builder.Services.AddScoped<IAdminApiService, AdminApiService>();

// Authentication
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();

// Radzen servislerini ekle
builder.Services.AddScoped<DialogService>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<TooltipService>();
builder.Services.AddScoped<ContextMenuService>();
await builder.Build().RunAsync();
