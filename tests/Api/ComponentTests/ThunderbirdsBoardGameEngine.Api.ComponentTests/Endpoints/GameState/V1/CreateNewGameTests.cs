using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http.Json;
using ThunderbirdsBoardGameEngine.Api.ComponentTests.Factories;
using ThunderbirdsBoardGameEngine.GameState.Contracts.Dtos.V1;
using ThunderbirdsBoardGameEngine.GameState.Contracts.Dtos.V1.CreateGame;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.KnownIdentities;
using ThunderbirdsBoardGameEngine.TestUtils.xUnit.Assertions;
using ThunderbirdsBoardGameEngine.TestUtils.xUnit.ClassData;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Api.ComponentTests.Endpoints.GameState.V1
{
    public class CreateNewGameTests : IClassFixture<ApiComponentWebApplicationFactory>
    {
        private readonly HttpClient _httpClient;

        private const int ApiVersion = 1;

        public CreateNewGameTests(ApiComponentWebApplicationFactory factory)
        {
            _httpClient = factory.CreateClient();
        }

        [Fact]
        public async Task CanCreateNewGameWithFourActiveCharacters()
        {
            // Arrange
            var turnOrder = new List<string>
            {
                KnownCharacterCodes.Scott.Value,
                KnownCharacterCodes.John.Value,
                KnownCharacterCodes.Virgil.Value,
                KnownCharacterCodes.Gordon.Value
            };

            var dto = new CreateGameRequestDto
            {
                TurnOrder = turnOrder
            };

            var route = GameStateRoutes.CreateGame();

            using var request = new HttpRequestMessage(HttpMethod.Post, route);
            request.Headers.Add("X-API-Version", ApiVersion.ToString());
            request.Content = JsonContent.Create(dto);

            // Act
            using var response = await _httpClient.SendAsync(request, TestContext.Current.CancellationToken);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var result = await response.Content.ReadFromJsonAsync<GameStateResponseDto>(cancellationToken: TestContext.Current.CancellationToken);
            Assert.NotNull(result);
            Assert.True(result.GameId != Guid.Empty);
            Assert.NotNull(result.ThunderbirdMachines);
            Assert.Equal(turnOrder, result.TurnOrder);

            var expectedLocation = GameStateRoutes.GetGame(result.GameId);

            Assert.NotNull(response.Headers.Location);
            Assert.Equal(expectedLocation, response.Headers.Location.AbsolutePath);
        }

        [Fact]
        public async Task CanCreateNewGameWithTwoActiveCharacters()
        {
            // Arrange
            var turnOrder = new List<string>
            {
                KnownCharacterCodes.Scott.Value,
                KnownCharacterCodes.John.Value
            };

            var dto = new CreateGameRequestDto
            {
                TurnOrder = turnOrder
            };

            var route = GameStateRoutes.CreateGame();

            using var request = new HttpRequestMessage(HttpMethod.Post, route);
            request.Headers.Add("X-API-Version", ApiVersion.ToString());
            request.Content = JsonContent.Create(dto);

            // Act
            using var response = await _httpClient.SendAsync(request, TestContext.Current.CancellationToken);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var result = await response.Content.ReadFromJsonAsync<GameStateResponseDto>(cancellationToken: TestContext.Current.CancellationToken);
            Assert.NotNull(result);
            Assert.True(result.GameId != Guid.Empty);
            Assert.NotNull(result.ThunderbirdMachines);
            Assert.Equal(turnOrder, result.TurnOrder);

            var expectedLocation = GameStateRoutes.GetGame(result.GameId);
            Assert.NotNull(response.Headers.Location);
            Assert.Equal(expectedLocation, response.Headers.Location.AbsolutePath);
        }

        [Fact]
        public async Task CannotCreateNewGameWithDuplicateCharacters()
        {
            // Arrange
            var dto = new CreateGameRequestDto
            {
                TurnOrder =
                [
                    KnownCharacterCodes.Scott.Value,
                    KnownCharacterCodes.John.Value,
                    KnownCharacterCodes.Scott.Value
                ]
            };

            var route = GameStateRoutes.CreateGame();

            using var request = new HttpRequestMessage(HttpMethod.Post, route);
            request.Headers.Add("X-API-Version", ApiVersion.ToString());
            request.Content = JsonContent.Create(dto);

            // Act
            using var response = await _httpClient.SendAsync(request, TestContext.Current.CancellationToken);

            // Assert
            Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);

            var result = await response.Content.ReadFromJsonAsync<ProblemDetails>(cancellationToken: TestContext.Current.CancellationToken);
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status422UnprocessableEntity, result.Status);
            Assert.Equal("Invalid turn order was detected.", result.Title);
        }

        [Fact]
        public async Task SingleCharacterReturnsBadRequest()
        {
            // Arrange
            var dto = new CreateGameRequestDto
            {
                TurnOrder =
                [
                    KnownCharacterCodes.Scott.Value
                ]
            };

            var route = GameStateRoutes.CreateGame();

            using var request = new HttpRequestMessage(HttpMethod.Post, route);
            request.Headers.Add("X-API-Version", ApiVersion.ToString());
            request.Content = JsonContent.Create(dto);

            // Act
            using var response = await _httpClient.SendAsync(request, TestContext.Current.CancellationToken);

            // Assert
            await ProblemDetailsAssertions.AssertBadRequestAsync(response, "Request validation failed.");
        }

        [Fact]
        public async Task EmptyTurnOrderReturnsBadRequest()
        {
            // Arrange
            var dto = new CreateGameRequestDto
            {
                TurnOrder = []
            };

            var route = GameStateRoutes.CreateGame();

            using var request = new HttpRequestMessage(HttpMethod.Post, route);
            request.Headers.Add("X-API-Version", ApiVersion.ToString());
            request.Content = JsonContent.Create(dto);

            // Act
            using var response = await _httpClient.SendAsync(request, TestContext.Current.CancellationToken);

            // Assert
            await ProblemDetailsAssertions.AssertBadRequestAsync(response, "Request validation failed.");
        }

        [Fact]
        public async Task FiveCharactersReturnsBadRequest()
        {
            // Arrange
            var dto = new CreateGameRequestDto
            {
                TurnOrder =
                [
                    KnownCharacterCodes.Scott.Value,
                    KnownCharacterCodes.John.Value,
                    KnownCharacterCodes.Virgil.Value,
                    KnownCharacterCodes.Gordon.Value,
                    KnownCharacterCodes.Alan.Value
                ]
            };

            var route = GameStateRoutes.CreateGame();

            using var request = new HttpRequestMessage(HttpMethod.Post, route);
            request.Headers.Add("X-API-Version", ApiVersion.ToString());
            request.Content = JsonContent.Create(dto);

            // Act
            using var response = await _httpClient.SendAsync(request, TestContext.Current.CancellationToken);

            // Assert
            await ProblemDetailsAssertions.AssertBadRequestAsync(response, "Request validation failed.");
        }

        [Fact]
        public async Task UnknownCharacterReturnsNotFound()
        {
            // Arrange
            var dto = new CreateGameRequestDto
            {
                TurnOrder =
                [
                    KnownCharacterCodes.Scott.Value,
                    KnownCharacterCodes.John.Value,
                    "UNKNOWN_CHARACTER"
                ]
            };

            var route = GameStateRoutes.CreateGame();

            using var request = new HttpRequestMessage(HttpMethod.Post, route);
            request.Headers.Add("X-API-Version", ApiVersion.ToString());
            request.Content = JsonContent.Create(dto);

            // Act
            using var response = await _httpClient.SendAsync(request, TestContext.Current.CancellationToken);

            // Assert
            await ProblemDetailsAssertions.AssertNotFoundAsync(response, "Character was not found.");
        }

        [Fact]
        public async Task MissingTurnOrderReturnsBadRequest()
        {
            // Arrange
            var dto = new CreateGameRequestDto
            {
                TurnOrder = null
            };

            var route = GameStateRoutes.CreateGame();

            using var request = new HttpRequestMessage(HttpMethod.Post, route);
            request.Headers.Add("X-API-Version", ApiVersion.ToString());
            request.Content = JsonContent.Create(dto);

            // Act
            using var response = await _httpClient.SendAsync(request, TestContext.Current.CancellationToken);

            // Assert
            await ProblemDetailsAssertions.AssertBadRequestAsync(response, "Request validation failed.");
        }

        [Fact]
        public async Task MissingRequestBodyReturnsBadRequest()
        {
            // Arrange
            CreateGameRequestDto dto = null;

            var route = GameStateRoutes.CreateGame();

            using var request = new HttpRequestMessage(HttpMethod.Post, route);
            request.Headers.Add("X-API-Version", ApiVersion.ToString());
            request.Content = JsonContent.Create(dto);

            // Act
            using var response = await _httpClient.SendAsync(request, TestContext.Current.CancellationToken);

            // Assert
            await ProblemDetailsAssertions.AssertBadRequestAsync(response, "Request validation failed.");
        }

        [Theory]
        [ClassData(typeof(NullOrWhitespaceStringData))]
        public async Task TurnOrderWithNullOrWhitespaceCharacterReturnsBadRequest(string? invalidCharacterCode)
        {
            // Arrange
            var dto = new CreateGameRequestDto
            {
                TurnOrder =
                [
                    KnownCharacterCodes.Scott.Value,
                    invalidCharacterCode,
                    KnownCharacterCodes.John.Value
                ]
            };

            var route = GameStateRoutes.CreateGame();

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
