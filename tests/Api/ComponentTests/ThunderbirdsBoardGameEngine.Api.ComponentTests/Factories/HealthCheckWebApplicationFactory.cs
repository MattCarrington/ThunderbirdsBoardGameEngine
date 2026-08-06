using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using ThunderbirdsBoardGameEngine.Api.ComponentTests.Fakes;

namespace ThunderbirdsBoardGameEngine.Api.ComponentTests.Factories
{
    public sealed class HealthCheckWebApplicationFactory : ApiComponentWebApplicationFactory
    {
        private readonly HealthStatus _databaseStatus;

        public HealthCheckWebApplicationFactory(HealthStatus databaseStatus)
        {
            _databaseStatus = databaseStatus;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            base.ConfigureWebHost(builder);

            builder.ConfigureTestServices(services =>
            {
                services.PostConfigure<HealthCheckServiceOptions>(options =>
                {
                    var existing = options.Registrations.Single(
                        registration =>
                            registration.Name == "game-state-database");

                    options.Registrations.Remove(existing);

                    options.Registrations.Add(
                        new HealthCheckRegistration(
                            "game-state-database",
                            _ => new StubHealthCheck(_databaseStatus),
                            failureStatus: HealthStatus.Unhealthy,
                            tags: ["readiness"]));
                });
            });
        }
    }
}
