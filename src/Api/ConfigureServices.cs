using System.Security.Claims;
using HashidsNet;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection.Auth;
using OpenTelemetry.Metrics;
using UrlShortenerService.Api.Middlewares;

namespace Microsoft.Extensions.DependencyInjection;

public static class ConfigureServices
{
    public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        _ = services.AddSingleton<ExceptionHandlingMiddleware>();

        _ = services.AddSingleton<IHashids>(
            new Hashids(
              salt: configuration["Hashids:Salt"],
              minHashLength: 6,
              alphabet: configuration["Hashids:Alphabet"])
            );
        _ = services.AddHttpContextAccessor();

        _ = services.AddHealthChecks();
        
        _ = services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();
        _ = services.AddAuthorization(options =>
        {
            options.AddPolicy(AuthData.WriterPolicy, p =>
            {
                p.RequireClaim("writer", "true");
                p.RequireClaim(ClaimTypes.Name);
            });
        });

        _ = services.AddOpenTelemetry()
            .WithMetrics(x =>
            {
                x.AddPrometheusExporter();
                x.AddMeter("Microsoft.AspNetCore.Http", "Requests", "Requests per second", "requests/second");
            });
        
        _ = services.AddFastEndpoints();

        // Customise default API behaviour
        _ = services.Configure<ApiBehaviorOptions>(options =>
                options.SuppressModelStateInvalidFilter = true);

        return services;
    }
}
