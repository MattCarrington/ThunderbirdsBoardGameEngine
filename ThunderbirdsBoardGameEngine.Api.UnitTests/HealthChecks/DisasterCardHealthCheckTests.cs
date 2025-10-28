using Microsoft.Extensions.Diagnostics.HealthChecks;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using ThunderbirdsBoardGameEngine.Api.HealthChecks;
using ThunderbirdsBoardGameEngine.Catalog.Application.Interfaces;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Entities;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Enums;
using ThunderbirdsBoardGameEngine.TestUtils.Builders;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Api.UnitTests.HealthChecks
{
    public class DisasterCardHealthCheckTests
    {
        [Theory]
        [InlineData(1)]
        [InlineData(int.MaxValue)]
        public async Task CheckHealthPoint_WhenDataAvailable_ReturnsHealthy(int probeCount)
        {
            // Arrange
            var probe = CreateProbe(probeCount);

            // Act
            var result = await PerformHealthCheck(probe);

            // Assert
            Assert.Equal(HealthStatus.Healthy, result.Status);
            Assert.Equal("Disaster Card data is available.", result.Description);
            Assert.Equal(probeCount, result.Data["count"]);
            Assert.Equal("v1.0.0", result.Data["version"]);
            Assert.Null(result.Exception);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(int.MinValue)]
        public async Task CheckHealthPoint_WhenDataUnavailable_ReturnsUnhealthy(int probeCount)
        {
            // Arrange
            var probe = CreateProbe(probeCount);

            // Act
            var result = await PerformHealthCheck(probe);

            // Assert
            Assert.Equal(HealthStatus.Unhealthy, result.Status);
            Assert.Equal("Disaster Card data is unavailable.", result.Description);
            Assert.Equal(probeCount, result.Data["count"]);
            Assert.Equal("v1.0.0", result.Data["version"]);
            Assert.Null(result.Exception);
        }

        [Fact]
        public async Task CheckHealthPoint_WhenServiceThrows_ReturnsUnhealthy()
        {
            // Arrange
            var probe = Substitute.For<IDisasterCardCatalogProbe>();
            probe.Count.Throws(new InvalidOperationException("Database error"));

            // Act
            var result = await PerformHealthCheck(probe);

            // Assert
            Assert.Equal(HealthStatus.Unhealthy, result.Status);
            Assert.Equal("Disaster card catalog check failed.", result.Description);
            Assert.IsType<InvalidOperationException>(result.Exception);
        }

        private static IDisasterCardCatalogProbe CreateProbe(int count)
        {
            var probe = Substitute.For<IDisasterCardCatalogProbe>();
            probe.Count.Returns(count);
            probe.Version.Returns("v1.0.0");
            return probe;
        }

        private async static Task<HealthCheckResult> PerformHealthCheck(IDisasterCardCatalogProbe probe)
        {
            var healthCheck = new DisasterCardCatalogHealthCheck(probe);

            var context = new HealthCheckContext()
            {
                Registration = new HealthCheckRegistration(
                    name: "catalog_readiness",
                    instance: healthCheck,
                    failureStatus: HealthStatus.Unhealthy,
                    tags: Array.Empty<string>())
            };

            return await healthCheck.CheckHealthAsync(context, CancellationToken.None);
        }
    }
}
