using System.Net;
using System.Net.Http.Json;
using ThunderbirdsBoardGameEngine.Api.ComponentTests.Factories;
using ThunderbirdsBoardGameEngine.GameState.Application;
using ThunderbirdsBoardGameEngine.GameState.Contracts.Dtos.V1;
using ThunderbirdsBoardGameEngine.GameState.Domain;
using ThunderbirdsBoardGameEngine.GameState.Domain.Setup.V1;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.KnownIdentities;
using ThunderbirdsBoardGameEngine.TestUtils.xUnit.Assertions;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Api.ComponentTests.Endpoints.GameState.V1
{
    public class GetGameTests : IClassFixture<ApiComponentWebApplicationFactory>
    {
        private readonly HttpClient _httpClient;
        private readonly IGameRepository _repository;

        private const int ApiVersion = 1;

        public GetGameTests(ApiComponentWebApplicationFactory factory)
        {
            _httpClient = factory.CreateClient();
            _repository = factory.Repository;
        }

        [Fact]
        public async Task CanGetExistingGame()
        {
            // Arrange
            var gameId = Guid.NewGuid();

            var game = new StandardGameSetupFactory().Create(gameId, DateTimeOffset.UtcNow);

            await _repository.CreateNewGameSession(game, TestContext.Current.CancellationToken);

            var route = $"/api/games/{gameId}";

            using var request = new HttpRequestMessage(HttpMethod.Get, route);
            request.Headers.Add("X-API-Version", ApiVersion.ToString());

            // Act
            using var response = await _httpClient.SendAsync(request, TestContext.Current.CancellationToken);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadFromJsonAsync<GameStateResponseDto>(cancellationToken: TestContext.Current.CancellationToken);
            Assert.NotNull(result);
            Assert.NotNull(result.ThunderbirdMachines);
        }

        [Fact]
        public async Task GetNonExistingGameReturnsNotFound()
        {
            // Arrange
            var gameId = Guid.NewGuid();

            var route = $"/api/games/{gameId}";

            using var request = new HttpRequestMessage(HttpMethod.Get, route);
            request.Headers.Add("X-API-Version", ApiVersion.ToString());

            // Act
            using var response = await _httpClient.SendAsync(request, TestContext.Current.CancellationToken);

            // Assert
            await ProblemDetailsAssertions.AssertNotFoundAsync(response, "Game not found.");
        }

        [Fact]
        public async Task GameSessionIsInvalidReturnsServerError()
        {
            // Arrange
            var gameId = Guid.NewGuid();

            var thunderbirds = new Dictionary<ThunderbirdCode, LocationCode>
            {
                { KnownThunderbirdCodes.Thunderbird1, new("the-moon") },
                { KnownThunderbirdCodes.Thunderbird2, new("the-moon") },
                { KnownThunderbirdCodes.Thunderbird3, new("the-moon") },
                { KnownThunderbirdCodes.Thunderbird4, new("the-moon") },
                { KnownThunderbirdCodes.Thunderbird5, new("the-moon") }
            };

            var characters = new Dictionary<CharacterCode, ThunderbirdCode>
            {
                { KnownCharacterCodes.Scott, KnownThunderbirdCodes.Thunderbird1 },
                { KnownCharacterCodes.Virgil, KnownThunderbirdCodes.Thunderbird2 },
                { KnownCharacterCodes.Alan, KnownThunderbirdCodes.Thunderbird3 },
                { KnownCharacterCodes.Gordon, KnownThunderbirdCodes.Thunderbird4 },
                { KnownCharacterCodes.John, KnownThunderbirdCodes.Thunderbird5 }
            };

            var game = Game.Create(gameId, DateTimeOffset.UtcNow, "invalid-setup", thunderbirds, characters);

            await _repository.CreateNewGameSession(game, TestContext.Current.CancellationToken);

            var route = $"/api/games/{gameId}";

            using var request = new HttpRequestMessage(HttpMethod.Get, route);
            request.Headers.Add("X-API-Version", ApiVersion.ToString());

            // Act
            using var response = await _httpClient.SendAsync(request, TestContext.Current.CancellationToken);

            // Assert
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        }

        [Fact]
        public async Task GetGameWithInvalidGuidReturnsNotFound()
        {
            // Arrange
            var invalidGameId = "invalid-guid";

            var route = $"/api/games/{invalidGameId}";

            using var request = new HttpRequestMessage(HttpMethod.Get, route);
            request.Headers.Add("X-API-Version", ApiVersion.ToString());

            // Act
            using var response = await _httpClient.SendAsync(request, TestContext.Current.CancellationToken);

            // Assert
            await ProblemDetailsAssertions.AssertNotFoundAsync(response, "Not Found");
        }

        [Fact]
        public async Task GetGameWithEmptyGuidReturnsBadRequest()
        {
            // Arrange
            var emptyGameId = Guid.Empty;

            var route = $"/api/games/{emptyGameId}";

            using var request = new HttpRequestMessage(HttpMethod.Get, route);
            request.Headers.Add("X-API-Version", ApiVersion.ToString());
            // Act

            using var response = await _httpClient.SendAsync(request, TestContext.Current.CancellationToken);

            // Assert
            var details = await ProblemDetailsAssertions.AssertBadRequestAsync(response, "Bad request.");

            Assert.Equal("The provided GUID is empty.", details.Detail);
        }
    }
}
