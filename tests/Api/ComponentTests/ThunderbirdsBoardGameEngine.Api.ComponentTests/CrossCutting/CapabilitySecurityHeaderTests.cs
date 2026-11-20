using System.Net;
using System.Net.Http.Json;
using ThunderbirdsBoardGameEngine.Api.ComponentTests.Endpoints.GameState;
using ThunderbirdsBoardGameEngine.Api.ComponentTests.Factories;
using ThunderbirdsBoardGameEngine.GameState.Contracts.Dtos.V1.CreateGame;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.KnownIdentities;
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
            var dto = new CreateGameRequestDto
            {
                TurnOrder = new List<string>
                {
                    KnownCharacterCodes.Scott.Value,
                    KnownCharacterCodes.John.Value,
                    KnownCharacterCodes.Virgil.Value,
                    KnownCharacterCodes.Gordon.Value
                }
            };

            var route = GameStateRoutes.CreateGame();

            using var request = new HttpRequestMessage(HttpMethod.Post, route);
            request.Content = JsonContent.Create(dto);

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

            var route = GameStateRoutes.GetGame(unknownGameId);

            using var request = new HttpRequestMessage(HttpMethod.Get, route);

            request.Headers.Add("X-API-Version", "1");

            using var response = await _client.SendAsync(request, TestContext.Current.CancellationToken);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

            Assert.Contains("no-store", response.Headers.CacheControl?.ToString());
        }

        [Fact]
        public async Task NonGameResponseDoesNotRequireNoStore()
        {
            var route = "/meta";    // Use the meta endpoint which is not part of the game API

            using var request = new HttpRequestMessage(HttpMethod.Get, route);

            request.Headers.Add("X-API-Version", "1");

            using var response = await _client.SendAsync(request, TestContext.Current.CancellationToken);

            Assert.NotEqual(true, response.Headers.CacheControl?.NoStore);

            AssertHeader(response, "Referrer-Policy", "no-referrer");
        }

        private static void AssertHeader(HttpResponseMessage response, string name, string expectedValue)
        {
            Assert.True(response.Headers.TryGetValues(name, out var values));

            Assert.Contains(expectedValue, values);
        }
    }
}