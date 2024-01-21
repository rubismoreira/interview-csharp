using UrlShortenerService.Application.Common.Interfaces;
using UrlShortenerService.Infrastructure.Persistence;
using UrlShortenerService.Infrastructure.Persistence.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using UrlShortenerService.Domain.Options;
using UrlShortenerService.Infrastructure.Services;

namespace Microsoft.Extensions.DependencyInjection;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<AuditableEntitySaveChangesInterceptor>();

        if (configuration.GetValue<bool>("UseInMemoryDatabase"))
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseInMemoryDatabase("UrlShortenerServiceDb"));
        }
        else
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
        }
        
        
        services.Configure<CacheOptions>(
            configuration.GetSection("Cache"));
        
        var cacheOptions = configuration.GetSection("Cache").Get<CacheOptions>();
        
        if (cacheOptions.UseRedis)
        {
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = cacheOptions.RedisServer;
            });
        }
       

        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

        services.AddScoped<ApplicationDbContextInitialiser>();

        _ = services.AddTransient<IDateTime, DateTimeService>();

        return services;
    }
}
