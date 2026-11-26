using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http.Json;
using ThunderbirdsBoardGameEngine.Api.ComponentTests.Factories;
using ThunderbirdsBoardGameEngine.GameState.Application;
using ThunderbirdsBoardGameEngine.GameState.Contracts.Dtos.V1;
using ThunderbirdsBoardGameEngine.GameState.Contracts.Dtos.V1.EndTurn;
using ThunderbirdsBoardGameEngine.GameState.Domain.Setup.V2;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.KnownIdentities;
using ThunderbirdsBoardGameEngine.TestUtils.xUnit.Assertions;
using ThunderbirdsBoardGameEngine.TestUtils.xUnit.ClassData;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Api.ComponentTests.Endpoints.GameState.V1
{
    public class EndTurnTests : IClassFixture<ApiComponentWebApplicationFactory>
    {
        private readonly HttpClient _httpClient;
        private readonly IGameRepository _repository;
        private const int ApiVersion = 1;

        public EndTurnTests(ApiComponentWebApplicationFactory factory)
        {
            _httpClient = factory.CreateClient();
            _repository = factory.Repository;
        }

        [Fact]
        public async Task CanEndTurnAndIncrementActiveTurnIndex()
        {
            // Arrange
            var gameId = Guid.NewGuid();

            var turnOrder = new List<CharacterCode>
            {
                KnownCharacterCodes.Gordon,
                KnownCharacterCodes.John,
                KnownCharacterCodes.LadyPenelope,
                KnownCharacterCodes.Scott,
            };

            var gameFactory = new StandardGameSetupFactory();

            var game = gameFactory.Create(gameId, DateTimeOffset.UtcNow, turnOrder);

            await _repository.CreateNewGameSession(game, TestContext.Current.CancellationToken);

            var dto = new EndTurnRequestDto
            {
                ExpectedActiveCharacterCode = KnownCharacterCodes.Gordon.Value
            };

            var route = GameStateRoutes.EndTurn(gameId);

            using var request = new HttpRequestMessage(HttpMethod.Post, route);
            request.Headers.Add("X-API-Version", ApiVersion.ToString());
            request.Content = JsonContent.Create(dto);

            // Act
            using var response = await _httpClient.SendAsync(request, TestContext.Current.CancellationToken);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadFromJsonAsync<GameStateResponseDto>(cancellationToken: TestContext.Current.CancellationToken);
            Assert.NotNull(result);
            Assert.Equal(gameId, result.GameId);
            Assert.Equal(KnownCharacterCodes.John.Value, result.ActiveCharacterCode);
        }

        [Fact]
        public async Task EndTurnOnNonExistingGameReturnsNotFound()
        {
            // Arrange
            var gameId = Guid.NewGuid();

            var dto = new EndTurnRequestDto
            {
                ExpectedActiveCharacterCode = KnownCharacterCodes.Gordon.Value
            };

            var route = GameStateRoutes.EndTurn(gameId);

            using var request = new HttpRequestMessage(HttpMethod.Post, route);
            request.Headers.Add("X-API-Version", ApiVersion.ToString());
            request.Content = JsonContent.Create(dto);

            // Act
            using var response = await _httpClient.SendAsync(request, TestContext.Current.CancellationToken);

            // Assert
            await ProblemDetailsAssertions.AssertNotFoundAsync(response, "Game not found.");
        }

        [Fact]
        public async Task EndTurnWithInvalidGuidReturnsNotFound()
        {
            // Arrange
            var invalidGameId = "invalid-guid";

            var dto = new EndTurnRequestDto
            {
                ExpectedActiveCharacterCode = KnownCharacterCodes.John.Value
            };

            var route = $"/api/games/{invalidGameId}/end-turn";

            using var request = new HttpRequestMessage(HttpMethod.Post, route);
            request.Headers.Add("X-API-Version", ApiVersion.ToString());
            request.Content = JsonContent.Create(dto);

            // Act
            using var response = await _httpClient.SendAsync(request, TestContext.Current.CancellationToken);

            // Assert
            await ProblemDetailsAssertions.AssertNotFoundAsync(response, "Not Found");
        }

        [Fact]
        public async Task ReturnsConflictWhenExpectedCharacterNotActiveCharacter()
        {
            // Arrange
            var gameId = Guid.NewGuid();

            var turnOrder = new List<CharacterCode>
            {
                KnownCharacterCodes.Gordon,
                KnownCharacterCodes.John
            };

            var gameFactory = new StandardGameSetupFactory();
            gameFactory.Create(gameId, DateTimeOffset.UtcNow, turnOrder);

            await _repository.CreateNewGameSession(gameFactory.Create(gameId, DateTimeOffset.UtcNow, turnOrder), TestContext.Current.CancellationToken);

            var dto = new EndTurnRequestDto
            {
                ExpectedActiveCharacterCode = KnownCharacterCodes.John.Value
            };

            var route = GameStateRoutes.EndTurn(gameId);

            using var request = new HttpRequestMessage(HttpMethod.Post, route);
            request.Headers.Add("X-API-Version", ApiVersion.ToString());
            request.Content = JsonContent.Create(dto);

            // Act
            using var response = await _httpClient.SendAsync(request, TestContext.Current.CancellationToken);

            // Assert
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);

            var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>(cancellationToken: TestContext.Current.CancellationToken);
            Assert.NotNull(problemDetails);
            Assert.Equal("Game state conflict.", problemDetails.Title);
            Assert.Equal("The game state has been modified by another process. Please refresh and try again.", problemDetails.Detail);
        }

        [Fact]
        public async Task EndTurnWithNullDtoReturnsBadRequest()
        {
            // Arrange
            var gameId = Guid.NewGuid();

            var route = GameStateRoutes.EndTurn(gameId);

            using var request = new HttpRequestMessage(HttpMethod.Post, route);
            request.Headers.Add("X-API-Version", ApiVersion.ToString());
            request.Content = JsonContent.Create((EndTurnRequestDto?)null);

            // Act
            using var response = await _httpClient.SendAsync(request, TestContext.Current.CancellationToken);

            // Assert
            await ProblemDetailsAssertions.AssertBadRequestAsync(response, "Request validation failed.");
        }

        [Theory]
        [ClassData(typeof(NullOrWhitespaceStringData))]
        public async Task EndTurnWithNullOrWhitespaceExpectedActiveCharacterCode(string? expectedActiveCharacterCode)
        {
            // Arrange
            var gameId = Guid.NewGuid();
            var turnOrder = new List<CharacterCode>
            {
                KnownCharacterCodes.Virgil,
                KnownCharacterCodes.Scott
            };

            var gameFactory = new StandardGameSetupFactory();
            gameFactory.Create(gameId, DateTimeOffset.UtcNow, turnOrder);

            await _repository.CreateNewGameSession(gameFactory.Create(gameId, DateTimeOffset.UtcNow, turnOrder), TestContext.Current.CancellationToken);

            var dto = new EndTurnRequestDto
            {
                ExpectedActiveCharacterCode = expectedActiveCharacterCode
            };

            var route = GameStateRoutes.EndTurn(gameId);

            using var request = new HttpRequestMessage(HttpMethod.Post, route);
            request.Headers.Add("X-API-Version", ApiVersion.ToString());
            request.Content = JsonContent.Create(dto);

            // Act
            using var response = await _httpClient.SendAsync(request, TestContext.Current.CancellationToken);

            // Assert
            await ProblemDetailsAssertions.AssertBadRequestAsync(response, "Request validation failed.");
        }
    }
}
