using System.Net;
using ThunderbirdsBoardGameEngine.Api.ComponentTests.Factories;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Api.ComponentTests.CrossCutting
{
    public sealed class CapabilitySecurityHeaderTests : IClassFixture<ApiComponentWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public CapabilitySecurityHeaderTests(ApiComponentWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GameApiResponseDisablesCachingAndReferrers()
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, "/api/games");

            request.Headers.Add("X-API-Version", "1");

            using var response = await _client.SendAsync(request, TestContext.Current.CancellationToken);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            Assert.Contains("no-store", response.Headers.CacheControl?.ToString());

            AssertHeader(response, "Referrer-Policy", "no-referrer");
        }

        [Fact]
        public async Task GameApiErrorResponseDisablesCaching()
        {
            var unknownGameId = Guid.NewGuid();

            using var request = new HttpRequestMessage(HttpMethod.Get, $"/api/games/{unknownGameId}");

            request.Headers.Add("X-API-Version", "1");

            using var response = await _client.SendAsync(request, TestContext.Current.CancellationToken);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

            Assert.Contains("no-store", response.Headers.CacheControl?.ToString());
        }

        [Fact]
        public async Task NonGameResponseDoesNotRequireNoStore()
        {
            using var response = await _client.GetAsync("/health/live", TestContext.Current.CancellationToken);

            Assert.DoesNotContain("no-store", response.Headers.CacheControl?.ToString() ?? string.Empty);

            AssertHeader(response, "Referrer-Policy", "no-referrer");
        }

        private static void AssertHeader(HttpResponseMessage response, string name, string expectedValue)
        {
            Assert.True(response.Headers.TryGetValues(name, out var values));

            Assert.Contains(expectedValue, values);
        }
    }
}