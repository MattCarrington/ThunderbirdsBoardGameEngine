using ThunderbirdsBoardGameEngine.GameState.Application.EndTurn;
using ThunderbirdsBoardGameEngine.GameState.ComponentTests.Helpers;
using ThunderbirdsBoardGameEngine.GameState.Domain.Setup.V2;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.KnownIdentities;
using Xunit;

namespace ThunderbirdsBoardGameEngine.GameState.ComponentTests.EndTurn
{
    public class EndTurnTests
    {
        [Fact]
        public async Task CanEndTurnAndIncrementActiveTurnIndex()
        {
            // Arrange
            await using var testHost = GameStateTestHost.CreateBuilder().Build();

            var gameId = Guid.NewGuid();

            var turnOrder = new List<CharacterCode>
            {
                KnownCharacterCodes.Gordon,
                KnownCharacterCodes.John,
                KnownCharacterCodes.LadyPenelope,
                KnownCharacterCodes.Scott,
            };

            var gameFactory = new StandardGameSetupFactory();
            var game = gameFactory.Create(gameId, DateTimeOffset.UtcNow, turnOrder);

            await testHost.Repository.CreateNewGameSession(game, TestContext.Current.CancellationToken);

            var command = new EndTurnCommand(gameId);
            // Act
            var result = await testHost.Mediator.Send(command, TestContext.Current.CancellationToken);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(gameId, result.GameSession.Id);
            Assert.Same(game, result.GameSession);
            Assert.Equal(1, result.GameSession.ActiveTurnIndex);
            Assert.Equal(KnownCharacterCodes.John, result.GameSession.ActiveCharacter);
        }
    }
}
