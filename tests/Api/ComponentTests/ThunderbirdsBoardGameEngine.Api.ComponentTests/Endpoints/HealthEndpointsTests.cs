using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Api.ComponentTests.Endpoints
{
    public class HealthEndpointsTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public HealthEndpointsTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetLiveness_WhenCalled_ReturnsOk()
        {
            // Arrange
            using var request = new HttpRequestMessage(HttpMethod.Get, "/health/live");

            // Act
            using var response = await _client.SendAsync(request);

            // Assert
            Assert.True(response.IsSuccessStatusCode); // Status Code 200-299
        }

        [Fact]
        public async Task GetReadiness_WhenCalled_ReturnsOk()
        {
            // Arrange
            using var request = new HttpRequestMessage(HttpMethod.Get, "/health/ready");

            // Act
            using var response = await _client.SendAsync(request);

            // Assert
            Assert.True(response.IsSuccessStatusCode); // Status Code 200-299
        }
    }
}
