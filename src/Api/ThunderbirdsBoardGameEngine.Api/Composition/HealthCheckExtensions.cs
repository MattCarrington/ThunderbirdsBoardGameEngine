using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using ThunderbirdsBoardGameEngine.Api.HealthChecks;

namespace ThunderbirdsBoardGameEngine.Api.Composition
{
    public static class HealthCheckExtensions
    {
        [Obsolete("Catalog runtime API is deprecated. Static game data is now provided by ReferenceData snapshots.")]
        public static IServiceCollection AddCatalogHealthChecks(this IServiceCollection services)
        {
            services.AddHealthChecks()
                .AddCheck<DisasterCardCatalogHealthCheck>(
                    name: "catalog_readiness",
                    failureStatus: HealthStatus.Unhealthy,
                    tags: new[] { "readiness" });

            return services;
        }

        public static IEndpointRouteBuilder MapApiHealthChecks(this IEndpointRouteBuilder endpoints)
        {
            // liveness: dependency-free
            endpoints.MapHealthChecks("/health/live", new HealthCheckOptions
            {
                Predicate = _ => false
            }).AllowAnonymous();

            // readiness: only checks tagged "readiness"
            endpoints.MapHealthChecks("/health/ready", new HealthCheckOptions
            {
                Predicate = r => r.Tags.Contains("readiness")
            }).AllowAnonymous();

            return endpoints;
        }
    }
}
