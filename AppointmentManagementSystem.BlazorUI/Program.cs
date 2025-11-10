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

// HttpClient yapılandırması - Blazor WebAssembly için HttpClientFactory kullan
builder.Services.AddHttpClient("API", client =>
{
    client.BaseAddress = new Uri("https://localhost:5089/");
    client.Timeout = TimeSpan.FromSeconds(30);
})
.AddHttpMessageHandler<AuthorizationMessageHandler>();

// AuthorizationMessageHandler'ı servis olarak ekle
builder.Services.AddScoped<AuthorizationMessageHandler>();

// HttpClient'ı primary olarak kullanmak için
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("API"));

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
