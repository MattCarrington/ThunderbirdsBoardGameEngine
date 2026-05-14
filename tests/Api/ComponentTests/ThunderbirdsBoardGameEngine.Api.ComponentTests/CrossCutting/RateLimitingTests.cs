using System.Net;
using System.Net.Http.Json;
using ThunderbirdsBoardGameEngine.Api.ComponentTests.Factories;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Api.ComponentTests.CrossCutting
{
    public class RateLimitingTests : IClassFixture<RateLimitingWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private const int ApiVersion = 1;

        public RateLimitingTests(RateLimitingWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task ReturnsTooManyRequestsWhenRateLimitIsExceeded()
        {
            // Arrange
            var route = $"/api/rules/rescue/towering-ocean/target";

            var dto = new
            {
                PresentDisasterBonusKeys = new[]
                {
                    "thunderbird-3",
                    "thunderbird-4"
                },
                PerformingCharacterKey = "gordon"
            };

            // Act - Send requests with version headers
            using var firstRequest = new HttpRequestMessage(HttpMethod.Post, route);
            firstRequest.Headers.Add("X-API-Version", ApiVersion.ToString());
            firstRequest.Content = JsonContent.Create(dto);
            var first = await _client.SendAsync(firstRequest);

            using var secondRequest = new HttpRequestMessage(HttpMethod.Post, route);
            secondRequest.Headers.Add("X-API-Version", ApiVersion.ToString());
            secondRequest.Content = JsonContent.Create(dto);
            var second = await _client.SendAsync(secondRequest);

            using var thirdRequest = new HttpRequestMessage(HttpMethod.Post, route);
            thirdRequest.Headers.Add("X-API-Version", ApiVersion.ToString());
            thirdRequest.Content = JsonContent.Create(dto);
            var third = await _client.SendAsync(thirdRequest);

            // Assert
            Assert.NotEqual(HttpStatusCode.TooManyRequests, first.StatusCode);
            Assert.NotEqual(HttpStatusCode.TooManyRequests, second.StatusCode);
            Assert.Equal(HttpStatusCode.TooManyRequests, third.StatusCode);
        }
    }
}
