using NSubstitute;
using ThunderbirdsBoardGameEngine.GameState.Application.CreateGame;
using ThunderbirdsBoardGameEngine.GameState.Application.Exceptions;
using ThunderbirdsBoardGameEngine.GameState.Domain;
using ThunderbirdsBoardGameEngine.GameState.Domain.Exceptions;
using ThunderbirdsBoardGameEngine.GameState.Domain.Setup.V2;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.KnownIdentities;
using Xunit;

namespace ThunderbirdsBoardGameEngine.GameState.Application.UnitTests.CreateGame
{
    public class CreateNewGameHandlerTests
    {
        [Fact]
        public async Task Handle_ShouldCreateNewGameSuccessfully_WhenValidationPassesAsync()
        {
            // Arrange
            var validator = Substitute.For<IGameStateIntegrityValidator>();
            var repository = Substitute.For<IGameRepository>();
            var existenceLookup = Substitute.For<ICharacterExistenceLookup>();
            existenceLookup.Exists(Arg.Any<CharacterCode>()).Returns(true);

            var handler = new CreateNewGameHandler(
                new StandardGameSetupFactory(),
                validator,
                repository,
                existenceLookup);

            var activeTurnOrder = new List<CharacterCode>
            {
                KnownCharacterCodes.Alan,
                KnownCharacterCodes.Gordon,
            };

            // Act
            var result = await handler.Handle(new CreateNewGameCommand(activeTurnOrder), TestContext.Current.CancellationToken);

            // Assert
            validator.Received(1)
                .Validate(Arg.Any<Game>());

            await repository.Received(1)
                .CreateNewGameSession(
                    Arg.Any<Game>(),
                    Arg.Any<CancellationToken>());

            Assert.NotNull(result);
            Assert.NotNull(result.GameSession);
        }

        [Fact]
        public async Task Handle_ShouldCreateGame_WhenFourActiveCharacters()
        {
            // Arrange
            var validator = Substitute.For<IGameStateIntegrityValidator>();
            var repository = Substitute.For<IGameRepository>();
            var existenceLookup = Substitute.For<ICharacterExistenceLookup>();
            existenceLookup.Exists(Arg.Any<CharacterCode>()).Returns(true);

            var handler = new CreateNewGameHandler(
                new StandardGameSetupFactory(),
                validator,
                repository,
                existenceLookup);

            var activeTurnOrder = new List<CharacterCode>
            {
                KnownCharacterCodes.Scott,
                KnownCharacterCodes.John,
                KnownCharacterCodes.LadyPenelope,
                KnownCharacterCodes.Virgil
            };

            // Act
            var result = await handler.Handle(new CreateNewGameCommand(activeTurnOrder), TestContext.Current.CancellationToken);

            // Assert
            validator.Received(1)
                .Validate(Arg.Any<Game>());

            await repository.Received(1)
                .CreateNewGameSession(
                    Arg.Any<Game>(),
                    Arg.Any<CancellationToken>());

            Assert.NotNull(result);
            Assert.NotNull(result.GameSession);
        }

        [Fact]
        public async Task Handle_ShouldThrowInvalidTurnOrderException_WhenTurnOrderHasLessThanTwoCharacters()
        {
            // Arrange
            var validator = Substitute.For<IGameStateIntegrityValidator>();
            var repository = Substitute.For<IGameRepository>();
            var existenceLookup = Substitute.For<ICharacterExistenceLookup>();
            existenceLookup.Exists(Arg.Any<CharacterCode>()).Returns(true);

            var handler = new CreateNewGameHandler(
                new StandardGameSetupFactory(),
                validator,
                repository,
                existenceLookup);
            var activeTurnOrder = new List<CharacterCode>

            {
                KnownCharacterCodes.Virgil,
            };

            // Act & Assert
            await Assert.ThrowsAsync<InvalidTurnOrderException>(
                () => handler.Handle(new CreateNewGameCommand(activeTurnOrder), TestContext.Current.CancellationToken));

            await repository.DidNotReceive()
                .CreateNewGameSession(
                    Arg.Any<Game>(),
                    Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_ShouldThrowInvalidTurnOrderException_WhenTurnOrderHasMoreThanFourCharacters()
        {
            // Arrange
            var validator = Substitute.For<IGameStateIntegrityValidator>();
            var repository = Substitute.For<IGameRepository>();

            var existenceLookup = Substitute.For<ICharacterExistenceLookup>();
            existenceLookup.Exists(Arg.Any<CharacterCode>()).Returns(true);

            var handler = new CreateNewGameHandler(
                new StandardGameSetupFactory(),
                validator,
                repository,
                existenceLookup);

            var activeTurnOrder = new List<CharacterCode>
            {
                new("character-1"),
                new("character-2"),
                new("character-3"),
                new("character-4"),
                new("character-5"),
            };

            // Act & Assert
            await Assert.ThrowsAsync<InvalidTurnOrderException>(
                () => handler.Handle(new CreateNewGameCommand(activeTurnOrder), TestContext.Current.CancellationToken));

            await repository.DidNotReceive()
                .CreateNewGameSession(
                    Arg.Any<Game>(),
                    Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_ShouldNotPersistGame_WhenIntegrityValidationFails()
        {
            // Arrange
            var validator = Substitute.For<IGameStateIntegrityValidator>();
            var repository = Substitute.For<IGameRepository>();

            var existenceLookup = Substitute.For<ICharacterExistenceLookup>();
            existenceLookup.Exists(Arg.Any<CharacterCode>()).Returns(true);

            validator
                .When(x => x.Validate(Arg.Any<Game>()))
                .Do(_ => throw new InvalidOperationException("Invalid game state."));

            var handler = new CreateNewGameHandler(
                new StandardGameSetupFactory(),
                validator,
                repository,
                existenceLookup);

            var activeTurnOrder = new List<CharacterCode>
            {
                KnownCharacterCodes.Scott,
                KnownCharacterCodes.John,
            };

            // Act
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => handler.Handle(new CreateNewGameCommand(activeTurnOrder), TestContext.Current.CancellationToken));

            // Assert
            validator.Received(1)
                .Validate(Arg.Any<Game>());

            await repository.DidNotReceive()
                .CreateNewGameSession(
                    Arg.Any<Game>(),
                    Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_ShouldThrowCharacterNotFoundException_WhenLookupReturnsFalse()
        {
            // Arrange
            var validator = Substitute.For<IGameStateIntegrityValidator>();
            var repository = Substitute.For<IGameRepository>();

            var existenceLookup = Substitute.For<ICharacterExistenceLookup>();
            existenceLookup.Exists(Arg.Any<CharacterCode>()).Returns(false);

            var handler = new CreateNewGameHandler(
                new StandardGameSetupFactory(),
                validator,
                repository,
                existenceLookup);

            var activeTurnOrder = new List<CharacterCode>
            {
                new("character-1"),
                new("character-2"),
            };

            // Act & Assert
            await Assert.ThrowsAsync<CharacterNotFoundException>(
                () => handler.Handle(new CreateNewGameCommand(activeTurnOrder), TestContext.Current.CancellationToken));

            await repository.DidNotReceive()
                .CreateNewGameSession(
                    Arg.Any<Game>(),
                    Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_ShouldThrowInvalidTurnOrderException_WhenDuplicateCharactersInTurnOrder()
        {
            // Arrange
            var validator = Substitute.For<IGameStateIntegrityValidator>();

            var repository = Substitute.For<IGameRepository>();

            var existenceLookup = Substitute.For<ICharacterExistenceLookup>();
            existenceLookup.Exists(Arg.Any<CharacterCode>()).Returns(true);

            var handler = new CreateNewGameHandler(
                new StandardGameSetupFactory(),
                validator,
                repository,
                existenceLookup);

            var activeTurnOrder = new List<CharacterCode>
            {
                KnownCharacterCodes.LadyPenelope,
                KnownCharacterCodes.LadyPenelope
            };

            // Act & Assert
            await Assert.ThrowsAsync<InvalidTurnOrderException>(
                () => handler.Handle(new CreateNewGameCommand(activeTurnOrder), TestContext.Current.CancellationToken));

            await repository.DidNotReceive()
                .CreateNewGameSession(
                    Arg.Any<Game>(),
                    Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_ShouldThrowInvalidTurnOrderException_WhenTurnOrderEmpty()
        {
            // Arrange
            var validator = Substitute.For<IGameStateIntegrityValidator>();

            var repository = Substitute.For<IGameRepository>();

            var existenceLookup = Substitute.For<ICharacterExistenceLookup>();
            existenceLookup.Exists(Arg.Any<CharacterCode>()).Returns(true);

            var handler = new CreateNewGameHandler(
                new StandardGameSetupFactory(),
                validator,
                repository,
                existenceLookup);

            var activeTurnOrder = new List<CharacterCode>(); // Empty turn order

            // Act & Assert
            await Assert.ThrowsAsync<InvalidTurnOrderException>(
                () => handler.Handle(new CreateNewGameCommand(activeTurnOrder), TestContext.Current.CancellationToken));

            await repository.DidNotReceive()
                .CreateNewGameSession(
                    Arg.Any<Game>(),
                    Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_ShouldThrowArgumentNullException_WhenTurnOrderNull()
        {
            // Arrange
            var validator = Substitute.For<IGameStateIntegrityValidator>();

            var repository = Substitute.For<IGameRepository>();

            var existenceLookup = Substitute.For<ICharacterExistenceLookup>();
            existenceLookup.Exists(Arg.Any<CharacterCode>()).Returns(true);

            var handler = new CreateNewGameHandler(
                new StandardGameSetupFactory(),
                validator,
                repository,
                existenceLookup);

            List<CharacterCode> activeTurnOrder = null!; // Null turn order

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(
                () => handler.Handle(new CreateNewGameCommand(activeTurnOrder), TestContext.Current.CancellationToken));

            await repository.DidNotReceive()
                .CreateNewGameSession(
                    Arg.Any<Game>(),
                    Arg.Any<CancellationToken>());
        }
    }
}
