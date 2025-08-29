using System.Reflection;
using HealthTech.Application.Common.Behaviors;
using HealthTech.Application.Common.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using MediatR;

namespace HealthTech.Application;

/// <summary>
/// Application layer dependency injection configuration
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Add application services to the service collection
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <returns>Service collection</returns>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        // Add MediatR
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));

        // Add FluentValidation
        services.AddValidatorsFromAssembly(assembly);

        // Add Pipeline Behaviors (order matters)
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CachingBehavior<,>));

        // Add Memory Cache for CachingBehavior
        services.AddMemoryCache();

        // Add AutoMapper (if needed in the future)
        // services.AddAutoMapper(assembly);

        return services;
    }
}
