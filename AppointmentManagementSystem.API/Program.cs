using AppointmentManagementSystem.API.Hubs;
using AppointmentManagementSystem.API.Middleware;
using AppointmentManagementSystem.API.Services;
using AppointmentManagementSystem.Application;
using AppointmentManagementSystem.Infrastructure;
using AppointmentManagementSystem.Infrastructure.Data;
using AppointmentManagementSystem.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Diagnostics;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Circular reference handling
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        
        // Büyük JSON response'lar için
        options.JsonSerializerOptions.MaxDepth = 64;
        
        // Null değerleri ignore et (response boyutunu küçült)
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });
builder.Services.AddEndpointsApiExplorer();

// Swagger with JWT support
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Appointment Management System API", Version = "v1" });

    // JWT Authentication
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});
// DbContext
//builder.Services.AddDbContext<AppointmentDbContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Infrastructure
builder.Services.AddInfrastructure(builder.Configuration.GetConnectionString("DefaultConnection"));

// Application
builder.Services.AddApplication();

// JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"] ?? throw new ArgumentNullException("JWT Key is missing"));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ClockSkew = TimeSpan.Zero // Token s�resi i�in tolerans s�f�r
    };
});

builder.Services.AddAuthorization();

// SignalR
builder.Services.AddSignalR();

// Notification Service
builder.Services.AddScoped<INotificationService, NotificationService>();

// CORS - Blazor için özel yapılandırma (SignalR için güncellenmiş)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazor",
        corsBuilder =>
        {
            corsBuilder.WithOrigins(
                "https://localhost:7172",  // Blazor HTTPS
                "http://localhost:5090",   // Blazor HTTP
                "https://localhost:5090" ,  // Blazor alternatif
                "https://aptivaplan.com.tr",  
                "http://aptivaplan.com.tr"   
            )
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials() // Authorization header ve SignalR için gerekli
            .WithExposedHeaders("*");
        });
});

// Auto-start Blazor UI service
//builder.Services.AddHostedService<BlazorAutoStartService>();

// Monthly Billing Service - Otomatik aylık ödeme
builder.Services.AddHostedService<MonthlyBillingService>();

// Kestrel server options - büyük response'lar için
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Limits.MaxRequestBodySize = 52428800; // 50 MB
    serverOptions.Limits.MaxResponseBufferSize = 52428800; // 50 MB
});

var app = builder.Build();

// Configure the HTTP request pipeline.
// Global exception handling middleware
app.UseMiddleware<ExceptionHandlingMiddleware>();

//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}

app.UseCors("AllowBlazor");
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
