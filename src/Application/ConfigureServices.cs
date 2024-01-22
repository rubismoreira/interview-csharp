using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using UrlShortenerService.Application.Common.Behaviours;
using UrlShortenerService.Application.Common.Repositories;

namespace UrlShortenerService.Application;

public static class ConfigureServices
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddMediatR(Assembly.GetExecutingAssembly());
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehaviour<,>));
        
       services.AddScoped<CacheMetrics>();
       services.AddTransient<DatabaseQueryMetrics>();

        services.AddScoped<IRedisCacheService, RedisCacheService>();
        services.AddScoped<IUrlRepository, UrlRepository>();

        return services;
    }
}
