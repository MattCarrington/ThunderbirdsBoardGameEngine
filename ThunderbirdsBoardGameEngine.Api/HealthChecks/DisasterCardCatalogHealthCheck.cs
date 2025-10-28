using Microsoft.Extensions.Diagnostics.HealthChecks;
using ThunderbirdsBoardGameEngine.Catalog.Application.Interfaces;

namespace ThunderbirdsBoardGameEngine.Api.HealthChecks
{
    public class DisasterCardCatalogHealthCheck : IHealthCheck
    {
        private readonly IDisasterCardCatalogProbe _disasterCardProbe;

        public DisasterCardCatalogHealthCheck(IDisasterCardCatalogProbe disasterCardProbe)
        {
            _disasterCardProbe = disasterCardProbe;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                var data = new Dictionary<string, object>
                {
                    ["version"] = _disasterCardProbe.Version,
                    ["count"] = _disasterCardProbe.Count
                };

                return await Task.FromResult(_disasterCardProbe.Count) > 0
                        ? HealthCheckResult.Healthy("Disaster Card data is available.", data: data)
                        : HealthCheckResult.Unhealthy("Disaster Card data is unavailable.", data: data);
            }
            catch (OperationCanceledException)
            {
                // Respect shutdown / timeouts: let the framework handle cancellation semantics.
                throw;
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy("Disaster card catalog check failed.", ex);                
            }
        }
    }
}
