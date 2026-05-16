using Xunit;

namespace ThunderbirdsBoardGameEngine.SmokeTests.Health
{
    public sealed class HealthEndpointTests : IDisposable
    {
        private readonly HttpClient _client;

        public HealthEndpointTests()
        {
            _client = new HttpClient
            {
                BaseAddress = new Uri(SmokeTestConfig.SmokeTestBaseUrl)
            };
        }

        [Fact]
        public async Task LivenessEndpointReturnsSuccess()
        {
            // Arrange

            // Act
            using var response = await _client.GetAsync("/health/live");

            // Assert
            Assert.True(response.IsSuccessStatusCode, $"Liveness check failed with status {response.StatusCode}");
        }

        [Fact]
        public async Task ReadinessEndpointReturnsSuccess()
        {
            // Arrange

            // Act
            using var response = await _client.GetAsync("/health/ready");

            // Assert
            Assert.True(response.IsSuccessStatusCode, $"Readiness check failed with status {response.StatusCode}");
        }

        public void Dispose()
        {
            _client?.Dispose();
        }
    }
}