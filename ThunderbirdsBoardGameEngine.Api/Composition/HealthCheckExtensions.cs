using Microsoft.Extensions.Diagnostics.HealthChecks;
using ThunderbirdsBoardGameEngine.Api.HealthChecks;

namespace ThunderbirdsBoardGameEngine.Api.Composition
{
    public static class HealthCheckExtensions
    {
        public static IServiceCollection AddCatalogHealthChecks(this IServiceCollection services)
        {
            services.AddHealthChecks()
                .AddCheck<DisasterCardCatalogHealthCheck>(
                    name: "catalog_readiness",
                    failureStatus: HealthStatus.Unhealthy,
                    tags: new[] { "readiness" });

            return services;
        }
    }
}
