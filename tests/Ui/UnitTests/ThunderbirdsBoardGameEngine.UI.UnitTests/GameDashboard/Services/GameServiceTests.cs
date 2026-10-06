using NSubstitute;
using System.Net;
using ThunderbirdsBoardGameEngine.Client.Core;
using ThunderbirdsBoardGameEngine.GameState.Client.Interfaces.V1;
using ThunderbirdsBoardGameEngine.GameState.Contracts.Dtos.V1;
using ThunderbirdsBoardGameEngine.GameState.Contracts.Dtos.V1.ThunderbirdMachines;
using ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Interfaces;
using ThunderbirdsBoardGameEngine.TestUtils.xUnit.ClassData;
using ThunderbirdsBoardGameEngine.UI.Features.GameDashboard.Mappers;
using ThunderbirdsBoardGameEngine.UI.Features.GameDashboard.Services;
using Xunit;

namespace ThunderbirdsBoardGameEngine.UI.UnitTests.GameDashboard.Services
{
    public class GameServiceTests
    {
        [Fact]
        public async Task CreateGameAsync_WhenCalled_CreatesGameSuccessfully()
        {
            // Arrange
            var dto = new GameStateResponseDto
            {
                GameId = Guid.NewGuid(),
                ThunderbirdMachines = []
            };

            var apiResult = ApiResult<GameStateResponseDto>.SuccessResult(dto, HttpStatusCode.Created);

            var client = Substitute.For<IGameClient>();
            client.CreateGameAsync(Arg.Any<CancellationToken>()).Returns(apiResult);

            var service = CreateGameService(client);

            // Act
            var game = await service.CreateGameAsync();

            // Assert
            Assert.False(game == Guid.Empty);

            await client.Received(1).CreateGameAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task CreateGameAsync_WhenClientReturnsError_ThrowsInvalidOperationException()
        {
            // Arrange
            var apiResult = ApiResult<GameStateResponseDto>.Failure("Error", HttpStatusCode.InternalServerError);

            var client = Substitute.For<IGameClient>();
            client.CreateGameAsync(Arg.Any<CancellationToken>()).Returns(apiResult);

            var service = CreateGameService(client);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => service.CreateGameAsync());
        }

        [Fact]
        public async Task CreateGameAsync_WhenClientReturnsNullData_ThrowsInvalidOperationException()
        {
            // Arrange
            var apiResult = ApiResult<GameStateResponseDto>.SuccessResult(null, HttpStatusCode.Created);

            var client = Substitute.For<IGameClient>();
            client.CreateGameAsync(Arg.Any<CancellationToken>()).Returns(apiResult);

            var service = CreateGameService(client);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => service.CreateGameAsync());
        }

        [Fact]
        public async Task GetGameAsync_WhenCalled_ReturnsGameSuccessfully()
        {
            // Arrange
            var gameId = Guid.NewGuid();

            var dto = new GameStateResponseDto
            {
                GameId = gameId,
                ThunderbirdMachines = []
            };

            var apiResult = ApiResult<GameStateResponseDto>.SuccessResult(dto, HttpStatusCode.OK);

            var client = Substitute.For<IGameClient>();
            client.GetGameStateAsync(gameId, Arg.Any<CancellationToken>()).Returns(apiResult);

            var service = CreateGameService(client);

            // Act
            var game = await service.GetGameAsync(gameId);

            // Assert
            Assert.NotNull(game);
            Assert.Equal(gameId, game.GameId);

            await client.Received(1).GetGameStateAsync(gameId, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task GetGameAsync_WhenClientReturnsNotFound_ReturnsNull()
        {
            // Arrange
            var gameId = Guid.NewGuid();

            var apiResult = ApiResult<GameStateResponseDto>.Failure("Not Found", HttpStatusCode.NotFound);

            var client = Substitute.For<IGameClient>();
            client.GetGameStateAsync(gameId, Arg.Any<CancellationToken>()).Returns(apiResult);

            var service = CreateGameService(client);

            // Act
            var game = await service.GetGameAsync(gameId);

            // Assert
            Assert.Null(game);
        }

        [Fact]
        public async Task GetGameAsync_WhenClientReturnsError_ThrowsInvalidOperationException()
        {
            // Arrange
            var gameId = Guid.NewGuid();

            var apiResult = ApiResult<GameStateResponseDto>.Failure("Error", HttpStatusCode.InternalServerError);

            var client = Substitute.For<IGameClient>();
            client.GetGameStateAsync(gameId, Arg.Any<CancellationToken>()).Returns(apiResult);

            var service = CreateGameService(client);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => service.GetGameAsync(gameId));
        }

        [Fact]
        public async Task GetGameAsync_WhenClientReturnsNullData_ThrowsInvalidOperationException()
        {
            // Arrange
            var gameId = Guid.NewGuid();

            var apiResult = ApiResult<GameStateResponseDto>.SuccessResult(null, HttpStatusCode.OK);

            var client = Substitute.For<IGameClient>();
            client.GetGameStateAsync(gameId, Arg.Any<CancellationToken>()).Returns(apiResult);

            var service = CreateGameService(client);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => service.GetGameAsync(gameId));
        }

        [Fact]
        public async Task MoveThunderbirdAsync_WhenCalled_ReturnsSuccessResult()
        {
            // Arrange
            var gameId = Guid.NewGuid();

            var thunderbirdCode = "TB1";

            var destinationLocationCode = "LOC1";

            var dto = new GameStateResponseDto
            {
                GameId = gameId,
                ThunderbirdMachines = []
            };

            var apiResult = ApiResult<GameStateResponseDto>.SuccessResult(dto, HttpStatusCode.OK);

            var client = Substitute.For<IGameClient>();
            client.MoveThunderbirdMachineAsync(gameId, thunderbirdCode, Arg.Any<MoveThunderbirdMachineRequestDto>(), Arg.Any<CancellationToken>()).Returns(apiResult);

            var service = CreateGameService(client);

            // Act
            var result = await service.MoveThunderbirdAsync(gameId, thunderbirdCode, destinationLocationCode);

            // Assert
            Assert.Equal(ThunderbirdMovementOutcome.Success, result.Outcome);
            await client.Received(1).MoveThunderbirdMachineAsync(gameId, thunderbirdCode, Arg.Any<MoveThunderbirdMachineRequestDto>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task MoveThunderbirdAsync_WhenClientReturnsUnprocessableEntity_ReturnsRejectedResult()
        {
            // Arrange
            var gameId = Guid.NewGuid();

            var thunderbirdCode = "TB1";

            var destinationLocationCode = "LOC1";

            var apiResult = ApiResult<GameStateResponseDto>.Failure("Unprocessable Entity", HttpStatusCode.UnprocessableEntity);

            var client = Substitute.For<IGameClient>();
            client.MoveThunderbirdMachineAsync(gameId, thunderbirdCode, Arg.Any<MoveThunderbirdMachineRequestDto>(), Arg.Any<CancellationToken>()).Returns(apiResult);

            var service = CreateGameService(client);

            // Act
            var result = await service.MoveThunderbirdAsync(gameId, thunderbirdCode, destinationLocationCode);

            // Assert
            Assert.Equal(ThunderbirdMovementOutcome.Rejected, result.Outcome);
        }

        [Fact]
        public async Task MoveThunderbirdAsync_WhenClientReturnsNotFound_ReturnsNotFoundResult()
        {
            // Arrange
            var gameId = Guid.NewGuid();

            var thunderbirdCode = "TB1";

            var destinationLocationCode = "LOC1";

            var apiResult = ApiResult<GameStateResponseDto>.Failure("Not Found", HttpStatusCode.NotFound);

            var client = Substitute.For<IGameClient>();
            client.MoveThunderbirdMachineAsync(gameId, thunderbirdCode, Arg.Any<MoveThunderbirdMachineRequestDto>(), Arg.Any<CancellationToken>()).Returns(apiResult);

            var service = CreateGameService(client);

            // Act
            var result = await service.MoveThunderbirdAsync(gameId, thunderbirdCode, destinationLocationCode);

            // Assert
            Assert.Equal(ThunderbirdMovementOutcome.NotFound, result.Outcome);
        }

        [Fact]
        public async Task MoveThunderbirdAsync_WhenClientReturnsError_ThrowsInvalidOperationException()
        {
            // Arrange
            var gameId = Guid.NewGuid();

            var thunderbirdCode = "TB1";

            var destinationLocationCode = "LOC1";

            var apiResult = ApiResult<GameStateResponseDto>.Failure("Error", HttpStatusCode.InternalServerError);

            var client = Substitute.For<IGameClient>();
            client.MoveThunderbirdMachineAsync(gameId, thunderbirdCode, Arg.Any<MoveThunderbirdMachineRequestDto>(), Arg.Any<CancellationToken>()).Returns(apiResult);

            var service = CreateGameService(client);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => service.MoveThunderbirdAsync(gameId, thunderbirdCode, destinationLocationCode));
        }

        [Theory]
        [ClassData(typeof(NullOrWhitespaceStringData))]
        public async Task MoveThunderbirdAsync_WhenDestinationLocationCodeIsNullOrWhitespace_ThrowsArgumentException(string? destinationLocationCode)
        {
            // Arrange
            var gameId = Guid.NewGuid();

            var thunderbirdCode = "TB1";

            var client = Substitute.For<IGameClient>();

            var service = CreateGameService(client);

            // Act & Assert
            await Assert.ThrowsAnyAsync<ArgumentException>(() => service.MoveThunderbirdAsync(gameId, thunderbirdCode, destinationLocationCode));
        }

        private static GameService CreateGameService(IGameClient client)
        {
            var thunderbirdCatalog = Substitute.For<IThunderbirdDefinitionCatalog>();
            var locationCatalog = Substitute.For<ILocationDefinitionCatalog>();
            var characterCatalog = Substitute.For<ICharacterDefinitionCatalog>();

            var mapper = new GameDashboardMapper(thunderbirdCatalog, locationCatalog, characterCatalog);

            return new GameService(client, mapper);
        }
    }
}
