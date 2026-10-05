using NSubstitute;
using System.Net;
using ThunderbirdsBoardGameEngine.Client.Core;
using ThunderbirdsBoardGameEngine.GameState.Client.Interfaces.V1;
using ThunderbirdsBoardGameEngine.GameState.Contracts.Dtos.V1;
using ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Interfaces;
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
