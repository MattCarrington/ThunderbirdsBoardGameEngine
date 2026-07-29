using NSubstitute;
using ThunderbirdsBoardGameEngine.GameState.Application.GetGame;
using ThunderbirdsBoardGameEngine.GameState.Domain;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
using Xunit;

namespace ThunderbirdsBoardGameEngine.GameState.Application.UnitTests.GetGame
{
    public class GetGameHandlerTests
    {
        [Fact]
        public async Task Handle_ShouldReturnGame_WhenGameExistsAsync()
        {
            // Arrange
            var gameId = Guid.NewGuid();
            var createdAtUtc = DateTime.UtcNow;
            var setupVersion = "v1";

            var characters = new Dictionary<CharacterCode, ThunderbirdCode>
            {
                { new CharacterCode("CHAR1"), new ThunderbirdCode("TB1") },
                { new CharacterCode("CHAR2"), new ThunderbirdCode("TB2") }
            };

            var machines = new Dictionary<ThunderbirdCode, LocationCode>
            {
                { new ThunderbirdCode("TB1"), new LocationCode("LOC1") },
                { new ThunderbirdCode("TB2"), new LocationCode("LOC2") }
            };

            var game = Game.Restore(gameId, createdAtUtc, setupVersion, machines, characters);

            var handler = CreateHandler(game);

            // Act
            var result = await handler.Handle(new GetGameQuery(gameId), CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Same(game, result.GameSession);
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenGameDoesNotExistAsync()
        {
            // Arrange
            var gameId = Guid.NewGuid();

            var handler = CreateHandler(null!);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => handler.Handle(new GetGameQuery(gameId), CancellationToken.None));
        }

        private GetGameHandler CreateHandler(Game game)
        {
            var gameRespository = Substitute.For<IGameRepository>();
            gameRespository.GetGameSessionById(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(game);

            return new GetGameHandler(gameRespository);
        }
    }
}
