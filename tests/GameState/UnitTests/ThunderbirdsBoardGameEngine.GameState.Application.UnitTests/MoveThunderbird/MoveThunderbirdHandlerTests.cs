using NSubstitute;
using ThunderbirdsBoardGameEngine.GameState.Application.Exceptions;
using ThunderbirdsBoardGameEngine.GameState.Application.MoveThunderbird;
using ThunderbirdsBoardGameEngine.GameState.Domain;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
using Xunit;

namespace ThunderbirdsBoardGameEngine.GameState.Application.UnitTests.MoveThunderbird
{
    public class MoveThunderbirdHandlerTests
    {
        [Fact]
        public async Task Handle_ShouldThrowException_WhenMovementIsInvalid()
        {
            // Arrange
            var game = CreateGame();

            var repository = CreateRepository(game);

            var decision = new ValidateMovementDecision(false, ["Movement is invalid due to game rules."]);

            var integrityValidator = CreateValidator();

            var handler = CreateHandler(repository, integrityValidator, decision);

            var command = CreateCommand();

            // Act & Assert
            await Assert.ThrowsAsync<ThunderbirdMovementRejectedException>(() => handler.Handle(command, TestContext.Current.CancellationToken));

            integrityValidator.DidNotReceive().Validate(Arg.Any<Game>());
            await repository.DidNotReceive().UpdateGameSession(Arg.Any<Game>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_ShouldUpdateGame_WhenMovementIsValid()
        {
            // Arrange
            var game = CreateGame();

            var repository = CreateRepository(game);

            var decision = new ValidateMovementDecision(true, Array.Empty<string>());

            var integrityValidator = CreateValidator();

            var handler = CreateHandler(repository, integrityValidator, decision);

            var command = CreateCommand();

            // Act
            var result = await handler.Handle(command, TestContext.Current.CancellationToken);

            // Assert
            Assert.NotNull(result);

            integrityValidator.Received(1).Validate(game);
            await repository.Received(1).UpdateGameSession(game, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_ShouldThrowGameNotFoundException_WhenGameSessionNotFound()
        {
            // Arrange
            var repository = CreateRepository(null!);

            var integrityValidator = CreateValidator();

            var decision = new ValidateMovementDecision(true, Array.Empty<string>());

            var handler = CreateHandler(repository, integrityValidator, decision);

            var command = CreateCommand();

            // Act & Assert
            await Assert.ThrowsAsync<GameNotFoundException>(() => handler.Handle(command, TestContext.Current.CancellationToken));

            integrityValidator.DidNotReceive().Validate(Arg.Any<Game>());
            await repository.DidNotReceive().UpdateGameSession(Arg.Any<Game>(), Arg.Any<CancellationToken>());
        }

        private static Game CreateGame()
        {
            return Game.Restore(
                id: Guid.NewGuid(),
                createdAtUtc: DateTimeOffset.UtcNow,
                setupVersion: "1.0",
                machines: new Dictionary<ThunderbirdCode, LocationCode>
                {
                    { new ThunderbirdCode("thunderbird-1"), new LocationCode("location-1") }
                },
                characters: new Dictionary<CharacterCode, ThunderbirdCode>()
            );
        }

        private static MoveThunderbirdCommand CreateCommand()
        {
            return new MoveThunderbirdCommand(
                GameId: Guid.NewGuid(),
                Thunderbird: new ThunderbirdCode("thunderbird-1"),
                Destination: new LocationCode("location-2")
            );
        }

        private static IGameRepository CreateRepository(Game game)
        {
            var repository = Substitute.For<IGameRepository>();
            repository.GetGameSessionById(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
                .Returns(game);

            return repository;
        }

        private static IGameStateIntegrityValidator CreateValidator()
        {
            var validator = Substitute.For<IGameStateIntegrityValidator>();

            return validator;
        }

        private static MoveThunderbirdHandler CreateHandler(IGameRepository repository, IGameStateIntegrityValidator integrityValidator, ValidateMovementDecision decision)
        {
            var gateway = Substitute.For<IValidateMovementGateway>();
            gateway.ValidateMovement(Arg.Any<ThunderbirdCode>(), Arg.Any<LocationCode>(), Arg.Any<LocationCode>(), Arg.Any<CancellationToken>())
                .Returns(decision);

            return new MoveThunderbirdHandler(repository, gateway, integrityValidator);
        }
    }
}
