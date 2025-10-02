using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Configuration;

namespace ThunderbirdsBoardGameEngine.Api.Healthcheck
{
    public class JsonDataHealthCheck : IHealthCheck
    {
        private readonly IOptions<DisasterCardJsonOptions> _options;

        public JsonDataHealthCheck(IOptions<DisasterCardJsonOptions> options)
        {
            _options = options;
        }

        public Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            var path = _options.Value.FilePath;

            if (string.IsNullOrWhiteSpace(path))
            {
                return Task.FromResult(
                    HealthCheckResult.Unhealthy("FilePath not configured"));
            }

            if (!File.Exists(path))
            {
                return Task.FromResult(
                    HealthCheckResult.Unhealthy($"Test data missing at {path}"));
            }

            return Task.FromResult(
                HealthCheckResult.Healthy("Test data available"));
        }
    }
}