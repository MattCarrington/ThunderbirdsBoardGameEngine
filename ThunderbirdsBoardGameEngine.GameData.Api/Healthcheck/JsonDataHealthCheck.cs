using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using ThunderbirdsBoardGameEngine.GameData.Infrastructure.Configuration;

namespace ThunderbirdsBoardGameEngine.GameData.Api.Healthcheck
{
    public class JsonDataHealthCheck : IHealthCheck
    {
        private readonly IOptions<CardDataOptions> _options;

        public JsonDataHealthCheck(IOptions<CardDataOptions> options)
        {
            _options = options;
        }

        public Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            var path = _options.Value.DisasterCardsFilePath;

            if (string.IsNullOrWhiteSpace(path))
            {
                return Task.FromResult(
                    HealthCheckResult.Unhealthy("DisasterCardsFilePath not configured"));
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