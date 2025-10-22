﻿using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace AppointmentManagementSystem.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));
            services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
            services.AddAutoMapper(typeof(DependencyInjection).Assembly);
            return services;
        }
    }
}
