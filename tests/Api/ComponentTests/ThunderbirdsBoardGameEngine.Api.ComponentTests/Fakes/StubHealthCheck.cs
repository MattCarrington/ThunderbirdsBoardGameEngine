using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ThunderbirdsBoardGameEngine.Api.ComponentTests.Fakes
{
    internal sealed class StubHealthCheck : IHealthCheck
    {
        private readonly HealthStatus _status;

        public StubHealthCheck(HealthStatus status)
        {
            _status = status;
        }

        public Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            var result = _status switch
            {
                HealthStatus.Healthy => HealthCheckResult.Healthy(),
                HealthStatus.Degraded => HealthCheckResult.Degraded(),
                _ => HealthCheckResult.Unhealthy()
            };

            return Task.FromResult(result);
        }
    }
}
