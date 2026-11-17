using ThunderbirdsBoardGameEngine.GameState.Application.GetGame;
using ThunderbirdsBoardGameEngine.GameState.ComponentTests.Helpers;
using ThunderbirdsBoardGameEngine.GameState.Domain.Setup.V2;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.KnownIdentities;
using Xunit;

namespace ThunderbirdsBoardGameEngine.GameState.ComponentTests.GetGame
{
    public class GetGameTests
    {
        [Fact]
        public async Task CanGetGameByIdAsync()
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

            var command = new GetGameQuery(gameId);

            // Act
            var result = await testHost.Mediator.Send(command, TestContext.Current.CancellationToken);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(gameId, result.GameSession.Id);
            Assert.Same(game, result.GameSession);
        }
    }
}
