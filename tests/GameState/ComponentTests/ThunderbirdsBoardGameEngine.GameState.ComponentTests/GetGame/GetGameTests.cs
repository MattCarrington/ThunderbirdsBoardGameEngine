using ThunderbirdsBoardGameEngine.GameState.Application.GetGame;
using ThunderbirdsBoardGameEngine.GameState.ComponentTests.Helpers;
using ThunderbirdsBoardGameEngine.GameState.Domain;
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

            var game = Game.Create(
                gameId,
                DateTimeOffset.UtcNow,
                "1.0.0",
                new Dictionary<ThunderbirdCode, LocationCode>
                {
                    { KnownThunderbirdCodes.Thunderbird1, KnownLocationCodes.SouthPacific },
                    { KnownThunderbirdCodes.Thunderbird2, KnownLocationCodes.Europe }
                },
                new Dictionary<CharacterCode, ThunderbirdCode>
                {
                    { KnownCharacterCodes.Scott, KnownThunderbirdCodes.Thunderbird1 },
                    { KnownCharacterCodes.Virgil, KnownThunderbirdCodes.Thunderbird2 }
                }
            );

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
