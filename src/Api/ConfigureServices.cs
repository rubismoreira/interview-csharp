using System.Reflection;
using System.Security.Claims;
using HashidsNet;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection.Auth;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using UrlShortenerService.Api.Middlewares;

namespace Api;

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
        
        _ = services.AddAutoMapper(Assembly.GetExecutingAssembly());
        
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
                x.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("Api"));
                x.AddPrometheusExporter();
                x.AddMeter("Microsoft.AspNetCore.Hosting",
                    "Microsoft.AspNetCore.Server.Kestrel", 
                    "UrlShortenerService.Cache",
                    "UrlShortenerService.Database");
                x.AddView("http.server.request.duration",
                    new ExplicitBucketHistogramConfiguration
                    {
                        Boundaries = new double[] { 0, 0.005, 0.01, 0.025, 0.05,
                            0.075, 0.1, 0.25, 0.5, 0.75, 1, 2.5, 5, 7.5, 10 }
                    });
            });
        
        _ = services.AddFastEndpoints();

        // Customise default API behaviour
        _ = services.Configure<ApiBehaviorOptions>(options =>
                options.SuppressModelStateInvalidFilter = true);

        return services;
    }
}
