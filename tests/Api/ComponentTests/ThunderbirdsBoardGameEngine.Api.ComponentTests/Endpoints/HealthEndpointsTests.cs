using Microsoft.Extensions.Diagnostics.HealthChecks;
using ThunderbirdsBoardGameEngine.Api.ComponentTests.Factories;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Api.ComponentTests.Endpoints
{
    public class HealthEndpointsTests
    {
        [Fact]
        public async Task GetLivenessReturnsOkWhenHealthStatusIsHealthy()
        {
            // Arrange
            var factory = new HealthCheckWebApplicationFactory(HealthStatus.Healthy);

            var client = factory.CreateClient();

            using var request = new HttpRequestMessage(HttpMethod.Get, "/health/live");

            // Act
            using var response = await client.SendAsync(request, TestContext.Current.CancellationToken);

            // Assert
            Assert.True(response.IsSuccessStatusCode); // Status Code 200-299
        }

        [Fact]
        public async Task GetReadinessReturnsOkWhenHealthStatusIsHealthy()
        {
            // Arrange
            var factory = new HealthCheckWebApplicationFactory(HealthStatus.Healthy);

            var client = factory.CreateClient();

            using var request = new HttpRequestMessage(HttpMethod.Get, "/health/ready");

            // Act
            using var response = await client.SendAsync(request, TestContext.Current.CancellationToken);

            // Assert
            Assert.True(response.IsSuccessStatusCode); // Status Code 200-299
        }

        [Fact]
        public async Task GetReadinessReturnsServiceUnavailableWhenHealthStatusIsUnhealthy()
        {
            // Arrange
            var factory = new HealthCheckWebApplicationFactory(HealthStatus.Unhealthy);

            var client = factory.CreateClient();

            using var request = new HttpRequestMessage(HttpMethod.Get, "/health/ready");

            // Act
            using var response = await client.SendAsync(request, TestContext.Current.CancellationToken);

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.ServiceUnavailable, response.StatusCode);
        }
    }
}
