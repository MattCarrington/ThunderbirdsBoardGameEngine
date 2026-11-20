using NSubstitute;
using ThunderbirdsBoardGameEngine.GameState.Application.EndTurn;
using ThunderbirdsBoardGameEngine.GameState.Application.Exceptions;
using ThunderbirdsBoardGameEngine.GameState.Domain;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
using Xunit;

namespace ThunderbirdsBoardGameEngine.GameState.Application.UnitTests.EndTurn
{
    public class EndTurnHandlerTests
    {
        [Fact]
        public async Task Handle_ShouldWrapAndPersistTurn_WhenActiveCharacterIsLast()
        {
            // Arrange
            var gameId = Guid.NewGuid();
            var game = CreateValidGame(gameId);

            var gameRespository = Substitute.For<IGameRepository>();
            gameRespository.GetGameSessionById(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(game);

            var integrityValidator = Substitute.For<IGameStateIntegrityValidator>();

            var handler = new EndTurnHandler(gameRespository, integrityValidator);

            // Act
            var result = await handler.Handle(new EndTurnCommand(gameId), CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Same(game, result.GameSession);
            Assert.Equal(0, game.ActiveTurnIndex);
            Assert.Equal(game.TurnOrder.First(), result.GameSession.ActiveCharacter);

            await gameRespository.Received(1).UpdateGameSession(game, Arg.Any<CancellationToken>());
            integrityValidator.Received(2).Validate(game);
        }

        [Fact]
        public async Task Handle_ShouldThrowGameNotFoundException_WhenGameDoesNotExistAsync()
        {
            // Arrange
            var gameId = Guid.NewGuid();

            var gameRespository = Substitute.For<IGameRepository>();
            gameRespository.GetGameSessionById(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((Game?)null);

            var integrityValidator = Substitute.For<IGameStateIntegrityValidator>();

            var handler = new EndTurnHandler(gameRespository, integrityValidator);

            // Act & Assert
            await Assert.ThrowsAsync<GameNotFoundException>(() => handler.Handle(new EndTurnCommand(gameId), CancellationToken.None));

            await gameRespository.DidNotReceive().UpdateGameSession(Arg.Any<Game>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_ShouldNotPersistGame_WhenIntegrityValidationFails()
        {
            // Arrange
            var gameId = Guid.NewGuid();
            var game = CreateValidGame(gameId);

            var gameRespository = Substitute.For<IGameRepository>();
            gameRespository.GetGameSessionById(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(game);

            var integrityValidator = Substitute.For<IGameStateIntegrityValidator>();
            integrityValidator.When(x => x.Validate(Arg.Any<Game>())).Do(x => throw new InvalidOperationException("Invalid game state"));

            var handler = new EndTurnHandler(gameRespository, integrityValidator);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(new EndTurnCommand(gameId), CancellationToken.None));
            await gameRespository.DidNotReceive().UpdateGameSession(Arg.Any<Game>(), Arg.Any<CancellationToken>());
        }

        private static Game CreateValidGame(Guid gameId)
        {
            var createdAtUtc = DateTime.UtcNow;
            var setupVersion = "v1";
            var stateVersion = 1;

            var characters = new Dictionary<CharacterCode, ThunderbirdCode>
            {
                { new CharacterCode("CHAR1"), new ThunderbirdCode("TB1") },
                { new CharacterCode("CHAR2"), new ThunderbirdCode("TB2") },
                { new CharacterCode("CHAR3"), new ThunderbirdCode("TB3") },
                { new CharacterCode("CHAR4"), new ThunderbirdCode("TB4") }
            };

            var machines = new Dictionary<ThunderbirdCode, LocationCode>
            {
                { new ThunderbirdCode("TB1"), new LocationCode("LOC1") },
                { new ThunderbirdCode("TB2"), new LocationCode("LOC2") }
            };

            var turnOrder = new List<CharacterCode> { new("CHAR1"), new("CHAR2"), new("CHAR3"), new("CHAR4") };

            return Game.Restore(gameId, createdAtUtc, setupVersion, stateVersion, machines, characters, turnOrder, 3);
        }
    }
}
