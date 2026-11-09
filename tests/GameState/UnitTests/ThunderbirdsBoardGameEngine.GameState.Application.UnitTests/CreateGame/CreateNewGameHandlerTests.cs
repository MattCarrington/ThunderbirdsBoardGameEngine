using NSubstitute;
using ThunderbirdsBoardGameEngine.GameState.Application.CreateGame;
using ThunderbirdsBoardGameEngine.GameState.Application.Exceptions;
using ThunderbirdsBoardGameEngine.GameState.Domain;
using ThunderbirdsBoardGameEngine.GameState.Domain.Exceptions;
using ThunderbirdsBoardGameEngine.GameState.Domain.Setup.V1;
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
                new("character-1"),
                new("character-2"),
            };

            // Act
            var result = await handler.Handle(new CreateNewGameCommand(activeTurnOrder), CancellationToken.None);

            // Assert
            validator.Received(1)
                .Validate(Arg.Any<Game>());

            await repository.Received(1)
                .CreateNewGameSession(
                    Arg.Any<Game>(),
                    Arg.Any<CancellationToken>());

            Assert.NotNull(result);
            Assert.NotNull(result.GameSession);
            Assert.Equal(0, result.GameSession.StateVersion);
            Assert.Equal(6, result.GameSession.Machines.Count);
            Assert.Equal(6, result.GameSession.Characters.Count);

            Assert.Equal(result.GameSession.Machines[KnownThunderbirdCodes.Thunderbird1], KnownLocationCodes.SouthPacific);
            Assert.Equal(result.GameSession.Machines[KnownThunderbirdCodes.Thunderbird2], KnownLocationCodes.SouthPacific);
            Assert.Equal(result.GameSession.Machines[KnownThunderbirdCodes.Thunderbird3], KnownLocationCodes.SouthPacific);
            Assert.Equal(result.GameSession.Machines[KnownThunderbirdCodes.Thunderbird4], KnownLocationCodes.SouthPacific);
            Assert.Equal(result.GameSession.Machines[KnownThunderbirdCodes.Thunderbird5], KnownLocationCodes.GeoStationaryOrbit);
            Assert.Equal(result.GameSession.Machines[KnownThunderbirdCodes.Fab1], KnownLocationCodes.Europe);

            Assert.Equal(result.GameSession.Characters[KnownCharacterCodes.Scott], KnownThunderbirdCodes.Thunderbird1);
            Assert.Equal(result.GameSession.Characters[KnownCharacterCodes.Virgil], KnownThunderbirdCodes.Thunderbird2);
            Assert.Equal(result.GameSession.Characters[KnownCharacterCodes.Alan], KnownThunderbirdCodes.Thunderbird3);
            Assert.Equal(result.GameSession.Characters[KnownCharacterCodes.Gordon], KnownThunderbirdCodes.Thunderbird4);
            Assert.Equal(result.GameSession.Characters[KnownCharacterCodes.John], KnownThunderbirdCodes.Thunderbird5);
            Assert.Equal(result.GameSession.Characters[KnownCharacterCodes.LadyPenelope], KnownThunderbirdCodes.Fab1);
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
                new("character-1"),
                new("character-2"),
                new("character-3"),
                new("character-4"),
            };

            // Act
            var result = await handler.Handle(new CreateNewGameCommand(activeTurnOrder), CancellationToken.None);

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
                new("character-1"),
            };

            // Act & Assert
            await Assert.ThrowsAsync<InvalidTurnOrderException>(
                () => handler.Handle(new CreateNewGameCommand(activeTurnOrder), CancellationToken.None));
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
                () => handler.Handle(new CreateNewGameCommand(activeTurnOrder), CancellationToken.None));
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
                new("character-1"),
                new("character-2"),
            };

            // Act
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => handler.Handle(new CreateNewGameCommand(activeTurnOrder), CancellationToken.None));

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
                () => handler.Handle(new CreateNewGameCommand(activeTurnOrder), CancellationToken.None));
        }
    }
}
