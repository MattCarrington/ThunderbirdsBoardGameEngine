using Humanizer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http.Json;
using ThunderbirdsBoardGameEngine.GameState.Contracts.Dtos.V1;
using ThunderbirdsBoardGameEngine.GameState.Contracts.Dtos.V1.ThunderbirdMachines;
using ThunderbirdsBoardGameEngine.GameState.WireMock.Stubs.V1;
using ThunderbirdsBoardGameEngine.TestUtils.WireMock.xUnit.Fixtures;
using WireMock.ResponseBuilders;
using WireMock.Server;
using Xunit;

namespace ThunderbirdsBoardGameEngine.GameState.WireMock.ComponentTests.Stubs.V1
{
    [Collection("WireMock")]
    public class GameStubTests
    {
        private readonly WireMockServer _server;
        private readonly GameStub _stub;

        public GameStubTests(WireMockFixture fixture)
        {
            _server = fixture.Host.WireMockServer;
            _server.Reset();

            _stub = fixture.Host.GameStub();
            _stub.RegisterMissingHeaderGuard();
            _stub.RegisterIncorrectHeaderGuard();
        }

        [Fact]
        public async Task CreateGameAsync_WhenSuccessSpecified_ReturnsSuccess()
        {
            // Arrange
            _stub.RegisterCreateGameSuccess();

            using var client = CreateClient();

            // Act
            using var response = await client.PostAsync(GameStub.CreateGameRoute, null, TestContext.Current.CancellationToken);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var result = await response.Content.ReadFromJsonAsync<GameStateResponseDto>(cancellationToken: TestContext.Current.CancellationToken);
            Assert.NotNull(result);
            AssertDefaultResult(result);
        }

        [Fact]
        public async Task CreateGameAsync_WhenGivenDto_ReturnsDto()
        {
            // Arrange
            var dto = CreateDto();

            _stub.RegisterCreateGameSuccess(dto);

            using var client = CreateClient();

            // Act
            using var response = await client.PostAsync(GameStub.CreateGameRoute, null, TestContext.Current.CancellationToken);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var result = await response.Content.ReadFromJsonAsync<GameStateResponseDto>(cancellationToken: TestContext.Current.CancellationToken);
            Assert.NotNull(result);
            AssertCreatedDto(dto, result);
        }

        [Fact]
        public async Task CreateGameAsync_WhenErrorSpecified_ReturnsError()
        {
            // Arrange
            _stub.RegisterCreateGameError(HttpStatusCode.BadRequest, "Invalid request", "The request was invalid.");

            using var client = CreateClient();

            // Act
            using var response = await client.PostAsync(GameStub.CreateGameRoute, null, TestContext.Current.CancellationToken);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            var result = await response.Content.ReadFromJsonAsync<ProblemDetails>(cancellationToken: TestContext.Current.CancellationToken);
            Assert.NotNull(result);
            Assert.Equal("Invalid request", result.Title);
            Assert.Equal("The request was invalid.", result.Detail);
        }

        [Fact]
        public async Task CreateGameAsync_WhenErrorRegistered_ReturnsDefaultError()
        {
            // Arrange
            _stub.RegisterCreateGameError();

            using var client = CreateClient();

            // Act
            using var response = await client.PostAsync(GameStub.CreateGameRoute, null, TestContext.Current.CancellationToken);

            // Assert
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);

            var result = await response.Content.ReadFromJsonAsync<ProblemDetails>(cancellationToken: TestContext.Current.CancellationToken);
            Assert.NotNull(result);
            Assert.Equal("An unexpected error occurred.", result.Title);
            Assert.Null(result.Detail);
        }

        [Fact]
        public async Task CreateGameAsync_WhenNoVersionHeader_ReturnsBadRequestAsync()
        {
            // Arrange
            using var client = new HttpClient { BaseAddress = new Uri(_server.Urls[0]) };

            _stub.RegisterCreateGameSuccess(); // should not be called

            // Act
            using var response = await client.PostAsync(GameStub.CreateGameRoute, null, TestContext.Current.CancellationToken);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>(cancellationToken: TestContext.Current.CancellationToken);
            Assert.NotNull(problem);
            Assert.Contains("Missing header", problem.Detail);
            Assert.Equal(StatusCodes.Status400BadRequest, problem.Status);
            Assert.Equal("Missing API version header", problem.Title);
        }

        [Fact]
        public async Task CreateGameAsync_WhenWrongVersionHeader_ReturnsBadRequestAsync()
        {
            // Arrange
            _stub.RegisterCreateGameSuccess(); // should not be called

            using var client = CreateClient("2.0");

            // Act
            using var response = await client.PostAsync(GameStub.CreateGameRoute, null, TestContext.Current.CancellationToken);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>(cancellationToken: TestContext.Current.CancellationToken);
            Assert.NotNull(problem);
            Assert.Equal(StatusCodes.Status400BadRequest, problem.Status);
            Assert.Contains("Unsupported version in header", problem.Title);
        }

        [Fact]
        public async Task GetGameAsync_WhenSuccess_ReturnsOkAsync()
        {
            // Arrange
            _stub.RegisterGetGameStateSuccess();

            using var client = CreateClient();

            // Act
            using var response = await client.GetAsync(GameStub.GetGameStateRoute, TestContext.Current.CancellationToken);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadFromJsonAsync<GameStateResponseDto>(cancellationToken: TestContext.Current.CancellationToken);
            Assert.NotNull(result);
            AssertDefaultResult(result);
        }

        [Fact]
        public async Task GetGameAsync_WhenGivenDto_ReturnsDto()
        {
            // Arrange
            var dto = CreateDto();

            _stub.RegisterGetGameStateSuccess(dto);

            using var client = CreateClient();

            // Act
            using var response = await client.GetAsync(GameStub.GetGameStateRoute, TestContext.Current.CancellationToken);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadFromJsonAsync<GameStateResponseDto>(cancellationToken: TestContext.Current.CancellationToken);
            Assert.NotNull(result);
            AssertCreatedDto(dto, result);
        }

        [Fact]
        public async Task GetGameAsync_WhenNotFound_ReturnsNotFound()
        {
            // Arrange
            _stub.RegisterGetGameStateNotFound("Game not found.");

            using var client = CreateClient();

            // Act
            using var response = await client.GetAsync(GameStub.GetGameStateRoute, TestContext.Current.CancellationToken);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

            var result = await response.Content.ReadFromJsonAsync<ProblemDetails>(cancellationToken: TestContext.Current.CancellationToken);
            Assert.NotNull(result);
            Assert.Equal("Resource not found", result.Title);
            Assert.Equal("Game not found.", result.Detail);
        }

        [Fact]
        public async Task GetGameAsync_WhenDefaultNotFoundRegistered_ReturnsDefaultMessage()
        {
            // Arrange
            _stub.RegisterGetGameStateNotFound();

            using var client = CreateClient();

            // Act
            using var response = await client.GetAsync(GameStub.GetGameStateRoute, TestContext.Current.CancellationToken);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

            var result = await response.Content.ReadFromJsonAsync<ProblemDetails>(cancellationToken: TestContext.Current.CancellationToken);
            Assert.NotNull(result);
            Assert.Equal("Resource not found", result.Title);
            Assert.Equal("The specified game was not found.", result.Detail);
        }

        [Fact]
        public async Task GetGameAsync_WhenSpecificErrorRegistered_ReturnsError()
        {
            // Arrange
            _stub.RegisterGetGameStateError(HttpStatusCode.BadGateway, "Bad Gateway", "A bad gateway error occurred.");

            using var client = CreateClient();

            // Act
            using var response = await client.GetAsync(GameStub.GetGameStateRoute, TestContext.Current.CancellationToken);

            // Assert
            Assert.Equal(HttpStatusCode.BadGateway, response.StatusCode);

            var result = await response.Content.ReadFromJsonAsync<ProblemDetails>(cancellationToken: TestContext.Current.CancellationToken);
            Assert.NotNull(result);
            Assert.Equal("Bad Gateway", result.Title);
            Assert.Equal("A bad gateway error occurred.", result.Detail);
        }

        [Fact]
        public async Task GetGameAsync_WhenDefaultErrorRegistered_ReturnsDefault()
        {
            // Arrange
            _stub.RegisterGetGameStateError();

            using var client = CreateClient();

            // Act
            using var response = await client.GetAsync(GameStub.GetGameStateRoute, TestContext.Current.CancellationToken);

            // Assert
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);

            var result = await response.Content.ReadFromJsonAsync<ProblemDetails>(cancellationToken: TestContext.Current.CancellationToken);
            Assert.NotNull(result);
            Assert.Equal("An unexpected error occurred.", result.Title);
            Assert.Null(result.Detail);
        }

        [Fact]
        public async Task GetGameAsync_WhenNoVersionHeader_ReturnsBadRequestAsync()
        {
            // Arrange
            using var client = new HttpClient { BaseAddress = new Uri(_server.Urls[0]) };

            _stub.RegisterGetGameStateSuccess(); // should not be called

            // Act
            using var response = await client.GetAsync(GameStub.GetGameStateRoute, TestContext.Current.CancellationToken);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>(cancellationToken: TestContext.Current.CancellationToken);
            Assert.NotNull(problem);
            Assert.Contains("Missing header", problem.Detail);
            Assert.Equal(StatusCodes.Status400BadRequest, problem.Status);
            Assert.Equal("Missing API version header", problem.Title);
        }

        [Fact]
        public async Task GetGameAsync_WhenWrongVersionHeader_ReturnsBadRequestAsync()
        {
            // Arrange
            _stub.RegisterGetGameStateSuccess(); // should not be called

            using var client = CreateClient("2.0");

            // Act
            using var response = await client.GetAsync(GameStub.GetGameStateRoute, TestContext.Current.CancellationToken);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>(cancellationToken: TestContext.Current.CancellationToken);
            Assert.NotNull(problem);
            Assert.Equal(StatusCodes.Status400BadRequest, problem.Status);
            Assert.Contains("Unsupported version in header", problem.Title);
        }

        [Fact]
        public async Task MoveThunderbirdAsync_WhenSuccess_ReturnsOk()
        {
            // Arrange
            _stub.RegisterMoveThunderbirdMachineSuccess();

            var request = new MoveThunderbirdMachineRequestDto
            {
                Destination = "south-america"
            };

            using var client = CreateClient();

            // Act
            using var response = await client.PostAsJsonAsync(GameStub.MoveThunderbirdMachineRoute, request, TestContext.Current.CancellationToken);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadFromJsonAsync<GameStateResponseDto>(cancellationToken: TestContext.Current.CancellationToken);
            Assert.NotNull(result);
            AssertDefaultResult(result);
        }

        [Fact]
        public async Task MoveThunderbirdAsync_WhenGivenDto_ReturnsDto()
        {
            // Arrange
            var dto = CreateDto();

            _stub.RegisterMoveThunderbirdMachineSuccess(dto);

            var request = new MoveThunderbirdMachineRequestDto
            {
                Destination = "arctic"
            };

            using var client = CreateClient();

            // Act
            using var response = await client.PostAsJsonAsync(GameStub.MoveThunderbirdMachineRoute, request, TestContext.Current.CancellationToken);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadFromJsonAsync<GameStateResponseDto>(cancellationToken: TestContext.Current.CancellationToken);
            Assert.NotNull(result);
            AssertCreatedDto(dto, result);
        }

        [Fact]
        public async Task MoveThunderbirdAsync_WhenSpecifiedNotFoundErrorRegistered_ReturnsNotFound()
        {
            // Arrange
            _stub.RegisterMoveThunderbirdMachineNotFound("Game not found.");

            var request = new MoveThunderbirdMachineRequestDto
            {
                Destination = "arctic"
            };

            using var client = CreateClient();

            // Act
            using var response = await client.PostAsJsonAsync(GameStub.MoveThunderbirdMachineRoute, request, TestContext.Current.CancellationToken);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

            var result = await response.Content.ReadFromJsonAsync<ProblemDetails>(cancellationToken: TestContext.Current.CancellationToken);
            Assert.NotNull(result);
            Assert.Equal("Resource not found", result.Title);
            Assert.Equal("Game not found.", result.Detail);
        }

        [Fact]
        public async Task MoveThunderbirdAsync_WhenDefaultNotFoundErrorRegistered_ReturnsNotFound()
        {
            // Arrange
            _stub.RegisterMoveThunderbirdMachineNotFound();

            var request = new MoveThunderbirdMachineRequestDto
            {
                Destination = "arctic"
            };

            using var client = CreateClient();

            // Act
            using var response = await client.PostAsJsonAsync(GameStub.MoveThunderbirdMachineRoute, request, TestContext.Current.CancellationToken);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

            var result = await response.Content.ReadFromJsonAsync<ProblemDetails>(cancellationToken: TestContext.Current.CancellationToken);
            Assert.NotNull(result);
            Assert.Equal("Resource not found", result.Title);
            Assert.Equal("The specified Thunderbird machine was not found.", result.Detail);
        }

        [Fact]
        public async Task MoveThunderbirdAsync_SpecificInvalidMovementRegistered_ReturnsUnprocessableEntity()
        {
            // Arrange
            _stub.RegisterMoveThunderbirdMachineInvalidMovement("Thunderbird not found.");

            var request = new MoveThunderbirdMachineRequestDto
            {
                Destination = "arctic"
            };

            using var client = CreateClient();

            // Act
            using var response = await client.PostAsJsonAsync(GameStub.MoveThunderbirdMachineRoute, request, TestContext.Current.CancellationToken);

            // Assert
            Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);

            var result = await response.Content.ReadFromJsonAsync<ProblemDetails>(cancellationToken: TestContext.Current.CancellationToken);
            Assert.NotNull(result);
            Assert.Equal("Invalid movement", result.Title);
            Assert.Equal("Thunderbird not found.", result.Detail);
        }

        [Fact]
        public async Task MoveThunderbirdAsync_DefaultInvalidMovementRegistered_ReturnsUnprocessableEntity()
        {
            // Arrange
            _stub.RegisterMoveThunderbirdMachineInvalidMovement();

            var request = new MoveThunderbirdMachineRequestDto
            {
                Destination = "arctic"
            };

            using var client = CreateClient();

            // Act
            using var response = await client.PostAsJsonAsync(GameStub.MoveThunderbirdMachineRoute, request, TestContext.Current.CancellationToken);

            // Assert
            Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);

            var result = await response.Content.ReadFromJsonAsync<ProblemDetails>(cancellationToken: TestContext.Current.CancellationToken);
            Assert.NotNull(result);
            Assert.Equal("Invalid movement", result.Title);
            Assert.Equal("The requested movement is invalid.", result.Detail);
        }

        [Fact]
        public async Task MoveThunderbirdAsync_ErrorRegistered_ReturnsSpecifiedError()
        {
            // Arrange
            _stub.RegisterMoveThunderbirdMachineError(HttpStatusCode.BadRequest, "Bad request", "Thunderbird not found");

            var request = new MoveThunderbirdMachineRequestDto
            {
                Destination = "arctic"
            };

            using var client = CreateClient();

            // Act
            using var response = await client.PostAsJsonAsync(GameStub.MoveThunderbirdMachineRoute, request, TestContext.Current.CancellationToken);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            var result = await response.Content.ReadFromJsonAsync<ProblemDetails>(cancellationToken: TestContext.Current.CancellationToken);
            Assert.NotNull(result);
            Assert.Equal("Bad request", result.Title);
            Assert.Equal("Thunderbird not found", result.Detail);
        }

        [Fact]
        public async Task MoveThunderbirdAsync_DefaultErrorRegistered_ReturnsDefaultError()
        {
            // Arrange
            _stub.RegisterMoveThunderbirdMachineError();

            var request = new MoveThunderbirdMachineRequestDto
            {
                Destination = "arctic"
            };

            using var client = CreateClient();

            // Act
            using var response = await client.PostAsJsonAsync(GameStub.MoveThunderbirdMachineRoute, request, TestContext.Current.CancellationToken);

            // Assert
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);

            var result = await response.Content.ReadFromJsonAsync<ProblemDetails>(cancellationToken: TestContext.Current.CancellationToken);
            Assert.NotNull(result);
            Assert.Equal("An unexpected error occurred.", result.Title);
            Assert.Null(result.Detail);
        }

        [Fact]
        public async Task MoveThunderbirdAsync_WhenNoVersionHeader_ReturnsBadRequestAsync()
        {
            // Arrange
            using var client = new HttpClient { BaseAddress = new Uri(_server.Urls[0]) };

            _stub.RegisterMoveThunderbirdMachineSuccess(); // should not be called

            var request = new MoveThunderbirdMachineRequestDto
            {
                Destination = "arctic"
            };

            // Act
            using var response = await client.PostAsJsonAsync(GameStub.MoveThunderbirdMachineRoute, request, TestContext.Current.CancellationToken);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>(cancellationToken: TestContext.Current.CancellationToken);
            Assert.NotNull(problem);
            Assert.Contains("Missing header", problem.Detail);
            Assert.Equal(StatusCodes.Status400BadRequest, problem.Status);
            Assert.Equal("Missing API version header", problem.Title);
        }

        [Fact]
        public async Task MoveThunderbirdAsync_WhenWrongVersionHeader_ReturnsBadRequestAsync()
        {
            // Arrange
            _stub.RegisterMoveThunderbirdMachineSuccess(); // should not be called

            var request = new MoveThunderbirdMachineRequestDto
            {
                Destination = "arctic"
            };

            using var client = CreateClient("2.0");

            // Act
            using var response = await client.PostAsJsonAsync(GameStub.MoveThunderbirdMachineRoute, request, TestContext.Current.CancellationToken);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>(cancellationToken: TestContext.Current.CancellationToken);
            Assert.NotNull(problem);
            Assert.Equal(StatusCodes.Status400BadRequest, problem.Status);
            Assert.Contains("Unsupported version in header", problem.Title);
        }

        private static GameStateResponseDto CreateDto()
        {
            return new GameStateResponseDto
            {
                GameId = Guid.NewGuid(),
                ThunderbirdMachines =
                [
                    new() {
                        ThunderbirdCode = "thunderbird-x",
                        LocationCode = "arctic",
                        OccupantCharacterCodes = []
                    }
                ]
            };
        }

        private HttpClient CreateClient(string versionHeader)
        {
            var client = new HttpClient { BaseAddress = new Uri(_server.Urls[0]) };
            client.DefaultRequestHeaders.Add(GameStub.VersionHeader, versionHeader);

            return client;
        }

        private HttpClient CreateClient()
        {
            return CreateClient(GameStub.VersionValue);
        }

        private static void AssertDefaultResult(GameStateResponseDto result)
        {
            Assert.NotNull(result);
            Assert.False(Guid.Empty == result.GameId);
            Assert.Equal(2, result.ThunderbirdMachines.Count);

            var thunderbird1 = result.ThunderbirdMachines.First(t => t.ThunderbirdCode == "thunderbird-1");
            Assert.Equal("south-america", thunderbird1.LocationCode);
            var character = Assert.Single(thunderbird1.OccupantCharacterCodes);
            Assert.Equal("scott", character);

            var thunderbird2 = result.ThunderbirdMachines.First(t => t.ThunderbirdCode == "thunderbird-2");
            Assert.Equal("north-america", thunderbird2.LocationCode);
            Assert.Equal(2, thunderbird2.OccupantCharacterCodes.Count);
        }

        private static void AssertCreatedDto(GameStateResponseDto created, GameStateResponseDto result)
        {
            Assert.Equal(created.GameId, result.GameId);
            Assert.Equal(created.ThunderbirdMachines.Count, result.ThunderbirdMachines.Count);

            var machine = Assert.Single(result.ThunderbirdMachines);
            Assert.Equal("thunderbird-x", machine.ThunderbirdCode);
            Assert.Equal("arctic", machine.LocationCode);
            Assert.Empty(machine.OccupantCharacterCodes);
        }
    }
}
