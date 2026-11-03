using NSubstitute;
using ThunderbirdsBoardGameEngine.GameState.Application.Exceptions;
using ThunderbirdsBoardGameEngine.GameState.Application.TransferCharacters;
using ThunderbirdsBoardGameEngine.GameState.Domain;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
using Xunit;

namespace ThunderbirdsBoardGameEngine.GameState.Application.UnitTests.TransferCharacters
{
    public class TransferCharacterHandlerTests
    {
        [Fact]
        public async Task Handle_ShouldSendCurrentStateAndPersistUpdatedGame_WhenRequestIsValidSingleTransfer()
        {
            // Arrange
            var game = CreateGame();

            var repository = CreateRepository(game);

            var integrityValidator = Substitute.For<IGameStateIntegrityValidator>();

            var decision = CharacterTransferDecision.Permitted();

            IReadOnlyCollection<CurrentThunderbirdState>? capturedState = null;

            var gateway = Substitute.For<ITransferCharacterRulesGateway>();

            gateway
                .ValidateTransferCharactersAsync(
                    Arg.Do<IReadOnlyCollection<CurrentThunderbirdState>>(
                        state => capturedState = state),
                    Arg.Any<IReadOnlyCollection<CharacterAssignment>>(),
                    Arg.Any<CancellationToken>())
                .Returns(decision);

            var handler = CreateHandler(repository, integrityValidator, gateway);

            var command = new TransferCharacterCommand(
                GameId: game.Id,
                CharacterAssignments:
                [
                    new CharacterAssignment(
                        CharacterCode: new CharacterCode("character-1"),
                        DestinationThunderbirdCode: new ThunderbirdCode("thunderbird-2"))
                ]
            );

            // Act
            var result = await handler.Handle(command, TestContext.Current.CancellationToken);

            // Assert
            Assert.NotNull(result);
            Assert.Same(game, result.GameSession);
            Assert.Equal(new ThunderbirdCode("thunderbird-2"), game.Characters[new CharacterCode("character-1")]);

            integrityValidator.Received(2).Validate(game);
            await repository.Received(1).UpdateGameSession(game, Arg.Any<CancellationToken>());

            AssertStartingMachineState(capturedState);
        }

        [Fact]
        public async Task Handle_ShouldSendCurrentStateAndPersistUpdatedGame_WhenTransferIsValidReciprocalSwap()
        {
            // Arrange
            var game = CreateGame();

            var repository = CreateRepository(game);

            var integrityValidator = CreateValidator();

            var decision = CharacterTransferDecision.Permitted();

            IReadOnlyCollection<CurrentThunderbirdState>? capturedState = null;

            var gateway = Substitute.For<ITransferCharacterRulesGateway>();

            gateway
                .ValidateTransferCharactersAsync(
                    Arg.Do<IReadOnlyCollection<CurrentThunderbirdState>>(
                        state => capturedState = state),
                    Arg.Any<IReadOnlyCollection<CharacterAssignment>>(),
                    Arg.Any<CancellationToken>())
                .Returns(decision);

            var handler = CreateHandler(repository, integrityValidator, gateway);

            var command = new TransferCharacterCommand(
                GameId: game.Id,
                CharacterAssignments:
                [
                    new(
                        CharacterCode: new CharacterCode("character-1"),
                        DestinationThunderbirdCode: new ThunderbirdCode("thunderbird-2")),
                    new(
                        CharacterCode: new CharacterCode("character-2"),
                        DestinationThunderbirdCode: new ThunderbirdCode("thunderbird-1"))
                ]
            );

            // Act
            var result = await handler.Handle(command, TestContext.Current.CancellationToken);

            // Assert
            Assert.NotNull(result);
            Assert.Same(game, result.GameSession);
            Assert.Equal(new ThunderbirdCode("thunderbird-2"), game.Characters[new CharacterCode("character-1")]);
            Assert.Equal(new ThunderbirdCode("thunderbird-1"), game.Characters[new CharacterCode("character-2")]);

            integrityValidator.Received(2).Validate(game);
            await repository.Received(1).UpdateGameSession(game, Arg.Any<CancellationToken>());

            AssertStartingMachineState(capturedState);
        }

        [Fact]
        public async Task Handle_ShouldThrowGameNotFoundException_WhenGameSessionNotFound()
        {
            // Arrange
            var repository = CreateRepository(null!);

            var integrityValidator = CreateValidator();

            var decision = CharacterTransferDecision.Permitted();

            var gateway = CreateTransferCharacterGateway(decision);

            var handler = CreateHandler(repository, integrityValidator, gateway);

            var command = new TransferCharacterCommand(
                GameId: Guid.NewGuid(),
                CharacterAssignments:
                [
                    new(
                        CharacterCode: new CharacterCode("character-1"),
                        DestinationThunderbirdCode: new ThunderbirdCode("thunderbird-2"))
                ]
            );

            // Act & Assert
            await Assert.ThrowsAsync<GameNotFoundException>(() => handler.Handle(command, TestContext.Current.CancellationToken));

            integrityValidator.DidNotReceive().Validate(Arg.Any<Game>());
            await repository.DidNotReceive().UpdateGameSession(Arg.Any<Game>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_ShouldThrowInvalidCharacterTransferRequestException_WhenCharacterAssignmentsAreEmpty()
        {
            // Arrange
            var game = CreateGame();

            var repository = CreateRepository(game);

            var integrityValidator = CreateValidator();

            var decision = CharacterTransferDecision.Permitted();

            var gateway = CreateTransferCharacterGateway(decision);

            var handler = CreateHandler(repository, integrityValidator, gateway);

            var command = new TransferCharacterCommand(
                GameId: game.Id,
                CharacterAssignments: Array.Empty<CharacterAssignment>()
            );

            // Act & Assert
            await Assert.ThrowsAsync<InvalidCharacterTransferRequestException>(() => handler.Handle(command, TestContext.Current.CancellationToken));

            integrityValidator.Received(1).Validate(game);
            await repository.DidNotReceive().UpdateGameSession(Arg.Any<Game>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_ShouldThrowInvalidCharacterTransferRequestException_WhenMoreThanTwoCharacterAssignments()
        {
            // Arrange
            var game = CreateGame();

            var repository = CreateRepository(game);

            var integrityValidator = CreateValidator();

            var decision = CharacterTransferDecision.Permitted();

            var gateway = CreateTransferCharacterGateway(decision);

            var handler = CreateHandler(repository, integrityValidator, gateway);

            var command = new TransferCharacterCommand(
                GameId: game.Id,
                CharacterAssignments:
                [
                    new(
                        CharacterCode: new CharacterCode("character-1"),
                        DestinationThunderbirdCode: new ThunderbirdCode("thunderbird-2")),
                    new(
                        CharacterCode: new CharacterCode("character-2"),
                        DestinationThunderbirdCode: new ThunderbirdCode("thunderbird-1")),
                    new(
                        CharacterCode: new CharacterCode("character-3"),
                        DestinationThunderbirdCode: new ThunderbirdCode("thunderbird-2"))
                ]
            );

            // Act & Assert
            await Assert.ThrowsAsync<InvalidCharacterTransferRequestException>(() => handler.Handle(command, TestContext.Current.CancellationToken));

            integrityValidator.Received(1).Validate(game);
            await repository.DidNotReceive().UpdateGameSession(Arg.Any<Game>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_ShouldThrowCharacterTransferRejectedException_WhenTransferIsRejectedByGateway()
        {
            // Arrange
            var game = CreateGame();

            var repository = CreateRepository(game);

            var integrityValidator = CreateValidator();

            var decision = CharacterTransferDecision.Rejected(CharacterTransferRejectionReason.CharacterCapacityExceeded);

            var gateway = CreateTransferCharacterGateway(decision);

            var handler = CreateHandler(repository, integrityValidator, gateway);

            var command = new TransferCharacterCommand(
                GameId: game.Id,
                CharacterAssignments:
                [
                    new(
                        CharacterCode: new CharacterCode("character-1"),
                        DestinationThunderbirdCode: new ThunderbirdCode("thunderbird-2"))
                ]
            );

            // Act & Assert
            await Assert.ThrowsAsync<CharacterTransferRejectedException>(() => handler.Handle(command, TestContext.Current.CancellationToken));

            integrityValidator.Received(1).Validate(game);
            await repository.DidNotReceive().UpdateGameSession(Arg.Any<Game>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_ShouldNotPersistGame_WhenIntegrityValidatorThrowsAfterValidatingTransfer()
        {
            // Arrange
            var game = CreateGame();

            var repository = CreateRepository(game);

            var validationCallCount = 0;

            var integrityValidator = CreateValidator();

            integrityValidator
                .When(validator => validator.Validate(Arg.Any<Game>()))
                .Do(_ =>
                {
                    validationCallCount++;

                    if (validationCallCount == 2)
                    {
                        throw new InvalidGameStateException(
                            "The resulting game state is invalid.");
                    }
                });

            var decision = CharacterTransferDecision.Permitted();

            var gateway = CreateTransferCharacterGateway(decision);

            var handler = CreateHandler(repository, integrityValidator, gateway);

            var command = new TransferCharacterCommand(
                GameId: game.Id,
                CharacterAssignments:
                [
                    new CharacterAssignment(
                        CharacterCode: new CharacterCode("character-1"),
                        DestinationThunderbirdCode: new ThunderbirdCode("thunderbird-2"))
                ]
            );

            // Act & Assert
            await Assert.ThrowsAsync<InvalidGameStateException>(() => handler.Handle(command, TestContext.Current.CancellationToken));

            integrityValidator.Received(2).Validate(game);
            await repository.DidNotReceive().UpdateGameSession(Arg.Any<Game>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_ShouldThrowCharacterTransferRejectedException_WhenCharacterIsAssignedToSameThunderbird()
        {
            // Arrange
            var game = CreateGame();

            var repository = CreateRepository(game);

            var integrityValidator = CreateValidator();

            var decision = CharacterTransferDecision.Permitted();

            var gateway = CreateTransferCharacterGateway(decision);

            var handler = CreateHandler(repository, integrityValidator, gateway);

            var command = new TransferCharacterCommand(
                GameId: game.Id,
                CharacterAssignments:
                [
                    new CharacterAssignment(
                        CharacterCode: new CharacterCode("character-1"),
                        DestinationThunderbirdCode: new ThunderbirdCode("thunderbird-1")),
                    new CharacterAssignment(
                        CharacterCode: new CharacterCode("character-2"),
                        DestinationThunderbirdCode: new ThunderbirdCode("thunderbird-2"))
                ]
            );

            // Act & Assert
            await Assert.ThrowsAsync<CharacterTransferRejectedException>(() => handler.Handle(command, TestContext.Current.CancellationToken));

            integrityValidator.Received(1).Validate(game);
            await repository.DidNotReceive().UpdateGameSession(Arg.Any<Game>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_ShouldThrowInvalidCharacterTransferRequestException_WhenDuplicateCharacterAssignmentsExist()
        {
            // Arrange
            var game = CreateGame();

            var repository = CreateRepository(game);

            var integrityValidator = CreateValidator();

            var decision = CharacterTransferDecision.Permitted();

            var gateway = CreateTransferCharacterGateway(decision);

            var handler = CreateHandler(repository, integrityValidator, gateway);

            var command = new TransferCharacterCommand(
                GameId: game.Id,
                CharacterAssignments:
                [
                    new CharacterAssignment(
                        CharacterCode: new CharacterCode("character-1"),
                        DestinationThunderbirdCode: new ThunderbirdCode("thunderbird-2")),
                    new CharacterAssignment(
                        CharacterCode: new CharacterCode("character-1"),
                        DestinationThunderbirdCode: new ThunderbirdCode("thunderbird-3"))
                ]
            );

            // Act & Assert
            await Assert.ThrowsAsync<InvalidCharacterTransferRequestException>(() => handler.Handle(command, TestContext.Current.CancellationToken));

            integrityValidator.Received(1).Validate(game);
            await repository.DidNotReceive().UpdateGameSession(Arg.Any<Game>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_ShouldThrow_WhenThreeParticipatingMachines()
        {
            var game = CreateGame();

            var repository = CreateRepository(game);

            var integrityValidator = CreateValidator();

            var decision = CharacterTransferDecision.Permitted();

            var gateway = CreateTransferCharacterGateway(decision);

            var handler = CreateHandler(repository, integrityValidator, gateway);

            var command = new TransferCharacterCommand(
                GameId: game.Id,
                CharacterAssignments:
                [
                    new CharacterAssignment(
                        CharacterCode: new CharacterCode("character-1"),
                        DestinationThunderbirdCode: new ThunderbirdCode("thunderbird-2")),
                    new CharacterAssignment(
                        CharacterCode: new CharacterCode("character-2"),
                        DestinationThunderbirdCode: new ThunderbirdCode("thunderbird-3"))
                ]
            );

            // Act & Assert
            await Assert.ThrowsAsync<InvalidCharacterTransferRequestException>(() => handler.Handle(command, TestContext.Current.CancellationToken));

            integrityValidator.Received(1).Validate(game);
            await repository.DidNotReceive().UpdateGameSession(Arg.Any<Game>(), Arg.Any<CancellationToken>());
        }

        private static Game CreateGame()
        {
            return Game.Restore(
                id: Guid.NewGuid(),
                createdAtUtc: DateTimeOffset.UtcNow,
                setupVersion: "1.0",
                stateVersion: 1,
                machines: new Dictionary<ThunderbirdCode, LocationCode>
                {
                    { new ThunderbirdCode("thunderbird-1"), new LocationCode("location-1") },
                    { new ThunderbirdCode("thunderbird-2"), new LocationCode("location-1") },
                    { new ThunderbirdCode("thunderbird-3"), new LocationCode("location-1") }
                },
                characters: new Dictionary<CharacterCode, ThunderbirdCode>
                {
                      { new CharacterCode("character-1"), new ThunderbirdCode("thunderbird-1") },
                      { new CharacterCode("character-2"), new ThunderbirdCode("thunderbird-2") }
                }
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

        private static ITransferCharacterRulesGateway CreateTransferCharacterGateway(CharacterTransferDecision decision)
        {
            var transferCharacterGateway = Substitute.For<ITransferCharacterRulesGateway>();
            transferCharacterGateway.ValidateTransferCharactersAsync(
                    Arg.Any<IReadOnlyCollection<CurrentThunderbirdState>>(),
                    Arg.Any<IReadOnlyCollection<CharacterAssignment>>(),
                    Arg.Any<CancellationToken>())
                .Returns(decision);

            return transferCharacterGateway;
        }

        private static TransferCharacterHandler CreateHandler(IGameRepository repository, IGameStateIntegrityValidator integrityValidator, ITransferCharacterRulesGateway gateway)
        {
            return new TransferCharacterHandler(repository, integrityValidator, gateway);
        }

        private static void AssertStartingMachineState(IReadOnlyCollection<CurrentThunderbirdState>? state)
        {
            Assert.NotNull(state);
            Assert.Equal(2, state.Count);

            var thunderbird1 = Assert.Single(
                state,
                item =>
                    item.Thunderbird ==
                    new ThunderbirdCode("thunderbird-1"));

            Assert.Equal(new LocationCode("location-1"), thunderbird1.Location);

            Assert.Equal([new CharacterCode("character-1")], thunderbird1.CharactersAboard);

            var thunderbird2 = Assert.Single(
                state,
                item =>
                    item.Thunderbird ==
                    new ThunderbirdCode("thunderbird-2"));

            Assert.Equal(new LocationCode("location-1"), thunderbird2.Location);

            Assert.Equal([new CharacterCode("character-2")], thunderbird2.CharactersAboard);
        }
    }
}
