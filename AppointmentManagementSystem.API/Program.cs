using AppointmentManagementSystem.API.Middleware;
using AppointmentManagementSystem.API.Services;
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

// Auto-start Blazor UI service (disabled when serving from same port)
// builder.Services.AddHostedService<BlazorAutoStartService>();

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

// Serve Blazor static files
var blazorDistPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "blazor");
if (Directory.Exists(blazorDistPath))
{
    app.UseStaticFiles();
    app.UseBlazorFrameworkFiles();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// SPA fallback - Blazor routes
if (Directory.Exists(blazorDistPath))
{
    app.MapFallbackToFile("blazor/index.html");
}
else
{
    // If Blazor not built, show info message
    app.MapGet("/", () => Results.Content(@"
        <html>
            <head><title>Appointment Management System</title></head>
            <body style='font-family: Arial; padding: 40px; text-align: center;'>
                <h1>🚀 Appointment Management System API</h1>
                <p>Blazor UI henüz build edilmemiş.</p>
                <p><strong>Build komutu:</strong></p>
                <pre style='background: #f5f5f5; padding: 20px; border-radius: 8px;'>build-blazor.bat</pre>
                <p><strong>veya</strong></p>
                <pre style='background: #f5f5f5; padding: 20px; border-radius: 8px;'>dotnet publish ../AppointmentManagementSystem.BlazorUI -o ./wwwroot/blazor</pre>
                <hr style='margin: 40px 0;'>
                <p><a href='/swagger' style='color: #007bff; font-size: 18px;'>📚 Swagger API Documentation</a></p>
            </body>
        </html>
    ", "text/html"));
}
