using AppointmentManagementSystem.API.Middleware;
using AppointmentManagementSystem.Application;
using AppointmentManagementSystem.Infrastructure;
using AppointmentManagementSystem.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Diagnostics;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
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
builder.Services.AddDbContext<AppointmentDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

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
// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
// Global exception handling middleware
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("AllowAll");
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Auto-start Blazor UI in development
if (app.Environment.IsDevelopment())
{
    StartBlazorUI();
}

app.Run();

// Method to start Blazor UI automatically
static void StartBlazorUI()
{
    try
    {
        var blazorProjectPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "AppointmentManagementSystem.BlazorUI");
        
        if (Directory.Exists(blazorProjectPath))
        {
            Console.WriteLine("🚀 Starting Blazor UI...");
            
            var processStartInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = "run",
                WorkingDirectory = blazorProjectPath,
                UseShellExecute = true, // Opens in new window
                CreateNoWindow = false
            };

            Process.Start(processStartInfo);
            Console.WriteLine("✅ Blazor UI started successfully!");
            Console.WriteLine($"📁 Blazor Path: {blazorProjectPath}");
        }
        else
        {
            Console.WriteLine($"⚠️  Blazor project not found at: {blazorProjectPath}");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error starting Blazor UI: {ex.Message}");
    }
}
