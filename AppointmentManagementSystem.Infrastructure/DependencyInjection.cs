using AppointmentManagementSystem.Domain.Interfaces;
using AppointmentManagementSystem.Infrastructure.Data;
using AppointmentManagementSystem.Infrastructure.Repositories;
using AppointmentManagementSystem.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AppointmentManagementSystem.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<AppointmentDbContext>(options =>
                options.UseSqlServer(connectionString));

            // Repositories
            services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IBusinessRepository, BusinessRepository>();
            services.AddScoped<IServiceRepository, ServiceRepository>();
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            services.AddScoped<IAppointmentRepository, AppointmentRepository>();
            services.AddScoped<IBusinessUserRepository, BusinessUserRepository>();

            // Photo Repositories
            services.AddScoped<IBusinessPhotoRepository, BusinessPhotoRepository>();
            services.AddScoped<IEmployeePhotoRepository, EmployeePhotoRepository>();
            services.AddScoped<IServicePhotoRepository, ServicePhotoRepository>();
            services.AddScoped<IAppointmentPhotoRepository, AppointmentPhotoRepository>();

            // Document Repositories
            services.AddScoped<IEmployeeDocumentRepository, EmployeeDocumentRepository>();


            // Unit of Work
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Services
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

            return services;
        }
    }
}
