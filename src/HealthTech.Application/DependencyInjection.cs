using System.Reflection;
using HealthTech.Application.Common.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using FluentValidation;

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

        // Add AutoMapper (if needed in the future)
        // services.AddAutoMapper(assembly);

        return services;
    }
}
