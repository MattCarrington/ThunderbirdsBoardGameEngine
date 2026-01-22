using Microsoft.Extensions.Diagnostics.HealthChecks;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using ThunderbirdsBoardGameEngine.Api.HealthChecks;
using ThunderbirdsBoardGameEngine.Catalog.Application.Interfaces;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Enums;
using ThunderbirdsBoardGameEngine.TestUtils.Catalog.Helpers;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Api.UnitTests.HealthChecks
{
    public class CharacterDefinitionCatalogHealthCheckTests
    {
        [Fact]
        public async Task CheckHealthAsync_WhenValidProbe_ReturnsHealthyAsync()
        {
            // Arrange
            var keys = TestCharacters.ValidSix.Select(c => c.Key).ToList();
            ICharacterDefinitionReferenceSourceProbe probe = CreateProbe(keys);

            // Act
            var result = await PerformHealthCheck(probe);

            // Assert
            Assert.Equal(HealthStatus.Healthy, result.Status);
            Assert.Equal("Character catalog is complete.", result.Description);
            Assert.Equal(6, result.Data["definedCount"]);
            Assert.Equal("v1.0.0", result.Data["version"]);
            Assert.Null(result.Exception);
        }

        [Fact]
        public async Task CheckHealthAsync_WhenIncompleteProbe_ReturnsUnhealthyAsync()
        {
            // Arrange
            var keys = TestCharacters.ValidSix.Where(c => c.Key != Character.John).Select(c => c.Key).ToList();

            var probe = CreateProbe(keys);

            // Act
            var result = await PerformHealthCheck(probe);

            // Assert
            Assert.Equal(HealthStatus.Unhealthy, result.Status);
            Assert.Equal("Character catalog is incomplete.", result.Description);
            Assert.Equal(5, result.Data["definedCount"]);
            Assert.Equal("v1.0.0", result.Data["version"]);
            Assert.Null(result.Exception);
        }

        [Fact]
        public async Task CheckHealthAsync_WhenNoKeys_ReturnsUnhealthyAsync()
        {
            // Arrange
            var keys = Array.Empty<Character>();

            var probe = CreateProbe(keys);

            // Act
            var result = await PerformHealthCheck(probe);

            // Assert
            Assert.Equal(HealthStatus.Unhealthy, result.Status);
            Assert.Equal("Character catalog contains no data.", result.Description);
            Assert.Equal(0, result.Data["definedCount"]);
            Assert.Equal("v1.0.0", result.Data["version"]);
            Assert.Null(result.Exception);
        }

        [Fact]
        public async Task CheckHealthAsync_WhenServiceThrows_ReturnsUnhealthyAsync()
        {
            // Arrange
            var probe = Substitute.For<ICharacterDefinitionReferenceSourceProbe>();
            probe.Keys.Throws(new InvalidOperationException("Database error"));

            // Act
            var result = await PerformHealthCheck(probe);

            // Assert
            Assert.Equal(HealthStatus.Unhealthy, result.Status);
            Assert.Equal("Character catalog health check failed.", result.Description);
            Assert.IsType<InvalidOperationException>(result.Exception);
            Assert.Equal("Database error", result.Exception?.Message);
        }

        [Fact]
        public async Task CheckHealthAsync_WhenOperationCancelledException_ThrowsOperationCanceledException()
        {
            // Arrange
            var probe = Substitute.For<ICharacterDefinitionReferenceSourceProbe>();
            probe.Keys.Throws(new OperationCanceledException("Operation was cancelled"));

            // Act & Assert
            await Assert.ThrowsAsync<OperationCanceledException>(async () => await PerformHealthCheck(probe));
        }

        private static ICharacterDefinitionReferenceSourceProbe CreateProbe(IEnumerable<Character> keys)
        {
            var probe = Substitute.For<ICharacterDefinitionReferenceSourceProbe>();
            probe.Keys.Returns(keys);
            probe.Version.Returns("v1.0.0");
            return probe;
        }

        private async static Task<HealthCheckResult> PerformHealthCheck(ICharacterDefinitionReferenceSourceProbe probe)
        {
            var healthCheck = new CharacterDefinitionCatalogHealthCheck(probe);

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
