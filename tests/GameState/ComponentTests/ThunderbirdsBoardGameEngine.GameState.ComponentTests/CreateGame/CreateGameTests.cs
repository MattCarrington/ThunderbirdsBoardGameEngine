using ThunderbirdsBoardGameEngine.GameState.Application.CreateGame;
using ThunderbirdsBoardGameEngine.GameState.Application.Exceptions;
using ThunderbirdsBoardGameEngine.GameState.ComponentTests.Helpers;
using ThunderbirdsBoardGameEngine.GameState.Domain.Exceptions;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.KnownIdentities;
using Xunit;

namespace ThunderbirdsBoardGameEngine.GameState.ComponentTests.CreateGame
{
    public class CreateGameTests
    {
        [Fact]
        public async Task CanCreateValidNewGameWithFourCharactersIncludingJohn()
        {
            // Arrange
            await using var testHost = GameStateTestHost.CreateBuilder().Build();

            var activeTurnOrder = new List<CharacterCode>
            {
                KnownCharacterCodes.Gordon,
                KnownCharacterCodes.John,
                KnownCharacterCodes.LadyPenelope,
                KnownCharacterCodes.Scott,
            };

            var command = new CreateNewGameCommand(activeTurnOrder);

            // Act
            var result = await testHost.Mediator.Send(command, TestContext.Current.CancellationToken);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.GameSession);
            Assert.Equal(6, result.GameSession.Machines.Count);
            Assert.Equal(6, result.GameSession.Characters.Count);

            Assert.Equal(KnownLocationCodes.SouthPacific, result.GameSession.Machines[KnownThunderbirdCodes.Thunderbird1]);
            Assert.Equal(KnownLocationCodes.SouthPacific, result.GameSession.Machines[KnownThunderbirdCodes.Thunderbird2]);
            Assert.Equal(KnownLocationCodes.GeoStationaryOrbit, result.GameSession.Machines[KnownThunderbirdCodes.Thunderbird3]);
            Assert.Equal(KnownLocationCodes.SouthPacific, result.GameSession.Machines[KnownThunderbirdCodes.Thunderbird4]);
            Assert.Equal(KnownLocationCodes.GeoStationaryOrbit, result.GameSession.Machines[KnownThunderbirdCodes.Thunderbird5]);
            Assert.Equal(KnownLocationCodes.Europe, result.GameSession.Machines[KnownThunderbirdCodes.Fab1]);

            Assert.Equal(KnownThunderbirdCodes.Thunderbird1, result.GameSession.Characters[KnownCharacterCodes.Scott]);
            Assert.Equal(KnownThunderbirdCodes.Thunderbird2, result.GameSession.Characters[KnownCharacterCodes.Virgil]);
            Assert.Equal(KnownThunderbirdCodes.Thunderbird3, result.GameSession.Characters[KnownCharacterCodes.Alan]);
            Assert.Equal(KnownThunderbirdCodes.Thunderbird4, result.GameSession.Characters[KnownCharacterCodes.Gordon]);
            Assert.Equal(KnownThunderbirdCodes.Thunderbird5, result.GameSession.Characters[KnownCharacterCodes.John]);
            Assert.Equal(KnownThunderbirdCodes.Fab1, result.GameSession.Characters[KnownCharacterCodes.LadyPenelope]);

            Assert.Equal(activeTurnOrder, result.GameSession.TurnOrder);
            Assert.Equal(KnownCharacterCodes.Gordon, result.GameSession.ActiveCharacter);
            Assert.Equal(0, result.GameSession.ActiveTurnIndex);
        }

        [Fact]
        public async Task CanCreateValidNewGameWithTwoPlayersExcludingJohn()
        {
            // Arrange
            await using var testHost = GameStateTestHost.CreateBuilder().Build();

            var activeTurnOrder = new List<CharacterCode>
            {
                KnownCharacterCodes.Virgil,
                KnownCharacterCodes.Alan,
            };

            var command = new CreateNewGameCommand(activeTurnOrder);

            // Act
            var result = await testHost.Mediator.Send(command, TestContext.Current.CancellationToken);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.GameSession);
            Assert.Equal(6, result.GameSession.Machines.Count);
            Assert.Equal(6, result.GameSession.Characters.Count);
            Assert.Equal(KnownLocationCodes.SouthPacific, result.GameSession.Machines[KnownThunderbirdCodes.Thunderbird3]);
            Assert.Equal(activeTurnOrder, result.GameSession.TurnOrder);
            Assert.Equal(KnownCharacterCodes.Virgil, result.GameSession.ActiveCharacter);
            Assert.Equal(0, result.GameSession.ActiveTurnIndex);
        }

        [Fact]
        public async Task ThrowsExceptionWhenCreatingGameWithNoPlayers()
        {
            // Arrange
            await using var testHost = GameStateTestHost.CreateBuilder().Build();

            var activeTurnOrder = Array.Empty<CharacterCode>();

            var command = new CreateNewGameCommand(activeTurnOrder);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidTurnOrderException>(() => testHost.Mediator.Send(command, TestContext.Current.CancellationToken));
        }

        [Fact]
        public async Task ThrowsExceptionWhenCreatingGameWithDuplicatePlayers()
        {
            // Arrange
            await using var testHost = GameStateTestHost.CreateBuilder().Build();

            var activeTurnOrder = new List<CharacterCode>
            {
                KnownCharacterCodes.Scott,
                KnownCharacterCodes.Virgil,
                KnownCharacterCodes.Scott, // Duplicate
            };

            var command = new CreateNewGameCommand(activeTurnOrder);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidTurnOrderException>(() => testHost.Mediator.Send(command, TestContext.Current.CancellationToken));
        }

        [Fact]
        public async Task ThrowsExceptionWhenCreatingGameWithInvalidCharacter()
        {
            // Arrange
            await using var testHost = GameStateTestHost.CreateBuilder().Build();

            var activeTurnOrder = new List<CharacterCode>
            {
                KnownCharacterCodes.Scott,
                KnownCharacterCodes.Virgil,
                new CharacterCode("InvalidCharacter"), // Invalid character
            };

            var command = new CreateNewGameCommand(activeTurnOrder);

            // Act & Assert
            await Assert.ThrowsAsync<CharacterNotFoundException>(() => testHost.Mediator.Send(command, TestContext.Current.CancellationToken));
        }

        [Fact]
        public async Task ThrowsInvalidTurnOrderExceptionWhenCreatingGameWithSinglePlayer()
        {
            // Arrange
            await using var testHost = GameStateTestHost.CreateBuilder().Build();

            var activeTurnOrder = new List<CharacterCode>
            {
                KnownCharacterCodes.Scott,
            };

            var command = new CreateNewGameCommand(activeTurnOrder);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidTurnOrderException>(() => testHost.Mediator.Send(command, TestContext.Current.CancellationToken));
        }

        [Fact]
        public async Task ThrowsInvalidTurnOrderExceptionWhenCreatingGameWithFivePlayers()
        {
            // Arrange
            await using var testHost = GameStateTestHost.CreateBuilder().Build();
            var activeTurnOrder = new List<CharacterCode>
            {
                KnownCharacterCodes.Scott,
                KnownCharacterCodes.Virgil,
                KnownCharacterCodes.Alan,
                KnownCharacterCodes.Gordon,
                KnownCharacterCodes.John
            };

            var command = new CreateNewGameCommand(activeTurnOrder);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidTurnOrderException>(() => testHost.Mediator.Send(command, TestContext.Current.CancellationToken));
        }

        [Fact]
        public async Task ThrowsExceptionIfTurnOrderIsNull()
        {
            // Arrange
            await using var testHost = GameStateTestHost.CreateBuilder().Build();

            var command = new CreateNewGameCommand(null!);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => testHost.Mediator.Send(command, TestContext.Current.CancellationToken));
        }
    }
}
