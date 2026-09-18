using NSubstitute;
using System.Net;
using ThunderbirdsBoardGameEngine.Client.Core;
using ThunderbirdsBoardGameEngine.GameState.Client.Clients.V1;
using ThunderbirdsBoardGameEngine.GameState.Contracts.Dtos.V1;
using ThunderbirdsBoardGameEngine.GameState.Contracts.Dtos.V1.ThunderbirdMachines;
using ThunderbirdsBoardGameEngine.Rules.Client.UnitTests.Helpers;
using ThunderbirdsBoardGameEngine.TestUtils.Stubs;
using ThunderbirdsBoardGameEngine.TestUtils.xUnit.ClassData;
using Xunit;

namespace ThunderbirdsBoardGameEngine.GameState.Client.UnitTests
{
    public class GameClientTests
    {
        [Fact]
        public async Task CreateGameAsync_WhenCalled_ShouldCallCorrectEndpoint()
        {
            // Arrange
            var stubHandler = new StubHttpMessageHandler("{}", HttpStatusCode.OK);

            var gameId = Guid.NewGuid();

            var apiResult = CreateGameStateResponseApiResult(gameId);

            var gameClient = CreateGameClient(stubHandler, apiResult);

            // Act
            _ = await gameClient.CreateGameAsync(TestContext.Current.CancellationToken);

            // Assert
            Assert.Equal($"https://localhost:5001/api/games", stubHandler.CapturedRequest?.RequestUri?.ToString());
        }

        [Fact]
        public async Task CreateGameAsync_WhenCalled_ShouldCallResponseHandler()
        {
            // Arrange
            var gameId = Guid.NewGuid();

            var stubHandler = new StubHttpMessageHandler("{}", HttpStatusCode.OK);

            var httpClient = CreateHttpClient(stubHandler);

            var apiResult = CreateGameStateResponseApiResult(gameId);

            var handler = HttpResponseHandlerHelper.CreateMockHttpResponseHandler(apiResult);

            var client = new GameClient(httpClient, handler);

            // Act
            _ = await client.CreateGameAsync(TestContext.Current.CancellationToken);

            // Assert
            await handler.Received(1).HandleResponseAsync<GameStateResponseDto>(Arg.Any<HttpResponseMessage>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task CreateGameAsync_WhenCalled_ShouldReturnExpectedResult()
        {
            // Arrange
            var gameId = Guid.NewGuid();

            var apiResult = CreateGameStateResponseApiResult(gameId);

            var gameClient = CreateGameClient(apiResult);

            // Act            
            var result = await gameClient.CreateGameAsync(TestContext.Current.CancellationToken);

            // Assert
            Assert.Equal(apiResult, result);
        }

        [Theory]
        [ClassData(typeof(ErrorResponseClassData))]
        public async Task CreateGameAsync_WhenCalledWithErrorResponse_ShouldReturnExpectedResult(HttpStatusCode statusCode, string reasonPhrase)
        {
            // Arrange
            var stubHandler = new StubHttpMessageHandler("{}", statusCode);

            var apiResult = ApiResult<GameStateResponseDto>.Failure(reasonPhrase, statusCode);

            var gameClient = CreateGameClient(stubHandler, apiResult);

            // Act            
            var result = await gameClient.CreateGameAsync(TestContext.Current.CancellationToken);

            // Assert
            Assert.Equal(apiResult, result);
        }

        [Fact]
        public async Task GetGameStateAsync_WhenCalled_ShouldCallCorrectEndpoint()
        {
            // Arrange
            var stubHandler = new StubHttpMessageHandler("{}", HttpStatusCode.OK);

            var gameId = Guid.NewGuid();

            var apiResult = CreateGameStateResponseApiResult(gameId);

            var gameClient = CreateGameClient(stubHandler, apiResult);

            // Act
            _ = await gameClient.GetGameStateAsync(gameId, TestContext.Current.CancellationToken);

            // Assert
            Assert.Equal($"https://localhost:5001/api/games/{gameId}", stubHandler.CapturedRequest?.RequestUri?.ToString());
        }

        [Fact]
        public async Task GetGameStateAsync_WhenCalled_ShouldCallResponseHandler()
        {
            // Arrange
            var gameId = Guid.NewGuid();

            var stubHandler = new StubHttpMessageHandler("{}", HttpStatusCode.OK);

            var httpClient = CreateHttpClient(stubHandler);

            var apiResult = CreateGameStateResponseApiResult(gameId);

            var handler = HttpResponseHandlerHelper.CreateMockHttpResponseHandler(apiResult);

            var client = new GameClient(httpClient, handler);

            // Act
            _ = await client.GetGameStateAsync(gameId, TestContext.Current.CancellationToken);

            // Assert
            await handler.Received(1).HandleResponseAsync<GameStateResponseDto>(Arg.Any<HttpResponseMessage>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task GetGameStateAsync_WhenCalled_ShouldReturnExpectedResult()
        {
            // Arrange
            var gameId = Guid.NewGuid();

            var apiResult = CreateGameStateResponseApiResult(gameId);

            var gameClient = CreateGameClient(apiResult);

            // Act            
            var result = await gameClient.GetGameStateAsync(gameId, TestContext.Current.CancellationToken);

            // Assert
            Assert.Equal(apiResult, result);
        }

        [Theory]
        [ClassData(typeof(ErrorResponseClassData))]
        public async Task GetGameStateAsync_WhenCalledWithErrorResponse_ShouldReturnExpectedResult(HttpStatusCode statusCode, string reasonPhrase)
        {
            // Arrange
            var stubHandler = new StubHttpMessageHandler("{}", statusCode);

            var gameId = Guid.NewGuid();

            var apiResult = ApiResult<GameStateResponseDto>.Failure(reasonPhrase, statusCode);

            var gameClient = CreateGameClient(stubHandler, apiResult);

            // Act            
            var result = await gameClient.GetGameStateAsync(gameId, TestContext.Current.CancellationToken);

            // Assert
            Assert.Equal(apiResult, result);
        }

        [Fact]
        public async Task GetGameStateAsync_WhenGuidEmpty_ShouldThrowArgumentException()
        {
            // Arrange
            var gameClient = CreateGameClient(CreateGameStateResponseApiResult(Guid.NewGuid()));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
               gameClient.GetGameStateAsync(Guid.Empty, TestContext.Current.CancellationToken));

            Assert.Equal("Game ID cannot be empty. (Parameter 'gameId')", exception.Message);
        }

        [Fact]
        public async Task MoveThunderbirdMachineAsync_WhenCalled_ShouldCallCorrectEndpoint()
        {
            // Arrange
            var thunderbirdCode = "TB1";

            var moveThunderbirdMachineRequestDto = CreateMoveThunderbirdMachineRequestDto();

            var stubHandler = new StubHttpMessageHandler("{}", HttpStatusCode.OK);

            var gameId = Guid.NewGuid();

            var apiResult = CreateGameStateResponseApiResult(gameId);

            var gameClient = CreateGameClient(stubHandler, apiResult);

            // Act
            _ = await gameClient.MoveThunderbirdMachineAsync(gameId, thunderbirdCode, moveThunderbirdMachineRequestDto, TestContext.Current.CancellationToken);

            // Assert
            Assert.Equal($"https://localhost:5001/api/games/{gameId}/thunderbird-machines/{thunderbirdCode}/movements", stubHandler.CapturedRequest?.RequestUri?.ToString());
        }

        [Fact]
        public async Task MoveThunderbirdMachineAsync_WhenCalled_ShouldCallResponseHandler()
        {
            // Arrange
            var moveThunderbirdMachineRequestDto = CreateMoveThunderbirdMachineRequestDto();

            var gameId = Guid.NewGuid();

            var stubHandler = new StubHttpMessageHandler("{}", HttpStatusCode.OK);

            var httpClient = CreateHttpClient(stubHandler);

            var apiResult = CreateGameStateResponseApiResult(gameId);

            var handler = HttpResponseHandlerHelper.CreateMockHttpResponseHandler(apiResult);

            var client = new GameClient(httpClient, handler);

            // Act
            _ = await client.MoveThunderbirdMachineAsync(gameId, "TB1", moveThunderbirdMachineRequestDto, TestContext.Current.CancellationToken);

            // Assert
            await handler.Received(1).HandleResponseAsync<GameStateResponseDto>(Arg.Any<HttpResponseMessage>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task MoveThunderbirdMachineAsync_WhenCalled_ShouldReturnExpectedResult()
        {
            // Arrange
            var gameId = Guid.NewGuid();

            var apiResult = CreateGameStateResponseApiResult(gameId);

            var gameClient = CreateGameClient(apiResult);

            var moveThunderbirdMachineRequestDto = CreateMoveThunderbirdMachineRequestDto();

            // Act            
            var result = await gameClient.MoveThunderbirdMachineAsync(gameId, "TB1", moveThunderbirdMachineRequestDto, TestContext.Current.CancellationToken);

            // Assert
            Assert.Equal(apiResult, result);
        }

        [Theory]
        [ClassData(typeof(ErrorResponseClassData))]
        public async Task MoveThunderbirdMachineAsync_WhenCalledWithErrorResponse_ShouldReturnExpectedResult(HttpStatusCode statusCode, string reasonPhrase)
        {
            // Arrange
            var stubHandler = new StubHttpMessageHandler("{}", statusCode);

            var gameId = Guid.NewGuid();

            var apiResult = ApiResult<GameStateResponseDto>.Failure(reasonPhrase, statusCode);

            var gameClient = CreateGameClient(stubHandler, apiResult);

            var moveThunderbirdMachineRequestDto = CreateMoveThunderbirdMachineRequestDto();

            // Act            
            var result = await gameClient.MoveThunderbirdMachineAsync(gameId, "TB1", moveThunderbirdMachineRequestDto, TestContext.Current.CancellationToken);

            // Assert
            Assert.Equal(apiResult, result);
        }

        [Fact]
        public async Task MoveThunderbirdMachineAsync_WhenGuidEmpty_ShouldThrowArgumentException()
        {
            // Arrange
            var gameClient = CreateGameClient(CreateGameStateResponseApiResult(Guid.NewGuid()));

            var moveThunderbirdMachineRequestDto = CreateMoveThunderbirdMachineRequestDto();

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
                gameClient.MoveThunderbirdMachineAsync(Guid.Empty, "TB1", moveThunderbirdMachineRequestDto, TestContext.Current.CancellationToken));

            Assert.Equal("Game ID cannot be empty. (Parameter 'gameId')", exception.Message);
        }

        [Theory]
        [ClassData(typeof(NullOrWhitespaceStringData))]
        public async Task MoveThunderbirdMachineAsync_WhenThunderbirdCodeNullOrWhitespace_ShouldThrowArgumentException(string? thunderbirdCode)
        {
            // Arrange
            var gameId = Guid.NewGuid();

            var gameClient = CreateGameClient(CreateGameStateResponseApiResult(gameId));

            var moveThunderbirdMachineRequestDto = CreateMoveThunderbirdMachineRequestDto();

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
                gameClient.MoveThunderbirdMachineAsync(gameId, thunderbirdCode, moveThunderbirdMachineRequestDto, TestContext.Current.CancellationToken));
            Assert.Equal("Thunderbird code cannot be null or whitespace. (Parameter 'thunderbirdCode')", exception.Message);
        }

        private static ApiResult<GameStateResponseDto> CreateGameStateResponseApiResult(Guid gameId)
        {
            var gameStateResponseDto = new GameStateResponseDto
            {
                GameId = gameId,
                ThunderbirdMachines =
                [
                    new()
                    {
                        ThunderbirdCode = "TB1",
                        LocationCode = "LOC1",
                        OccupantCharacterCodes = ["CHAR1", "CHAR2"]
                    }
                ]
            };

            return ApiResult<GameStateResponseDto>.SuccessResult(gameStateResponseDto, HttpStatusCode.OK);
        }

        private static GameClient CreateGameClient(StubHttpMessageHandler stubHandler, ApiResult<GameStateResponseDto> apiResult)
        {
            var httpClient = CreateHttpClient(stubHandler);

            var responseHandler = HttpResponseHandlerHelper.CreateMockHttpResponseHandler(apiResult);

            return new GameClient(httpClient, responseHandler);
        }

        private static MoveThunderbirdMachineRequestDto CreateMoveThunderbirdMachineRequestDto()
        {
            return new MoveThunderbirdMachineRequestDto
            {
                Destination = "destination"
            };
        }

        private static GameClient CreateGameClient(ApiResult<GameStateResponseDto> apiResult)
        {
            var stubHandler = new StubHttpMessageHandler("{}", HttpStatusCode.OK);

            return CreateGameClient(stubHandler, apiResult);
        }

        private static HttpClient CreateHttpClient(StubHttpMessageHandler stubHandler)
        {
            return new HttpClient(stubHandler)
            {
                BaseAddress = new Uri("https://localhost:5001/")
            };
        }

        private class ErrorResponseClassData : TheoryData<HttpStatusCode, string>
        {
            public ErrorResponseClassData()
            {
                Add(HttpStatusCode.BadRequest, "Bad Request");
                Add(HttpStatusCode.Unauthorized, "Unauthorized");
                Add(HttpStatusCode.Forbidden, "Forbidden");
                Add(HttpStatusCode.NotFound, "Not Found");
                Add(HttpStatusCode.InternalServerError, "Internal Server Error");
                Add(HttpStatusCode.ServiceUnavailable, "Service Unavailable");
            }
        }
    }
}
