using ThunderbirdsBoardGameEngine.GameState.Application;
using ThunderbirdsBoardGameEngine.GameState.Application.Exceptions;
using ThunderbirdsBoardGameEngine.GameState.Application.MoveThunderbird;
using ThunderbirdsBoardGameEngine.GameState.ComponentTests.Helpers;
using ThunderbirdsBoardGameEngine.GameState.Domain;
using ThunderbirdsBoardGameEngine.GameState.Domain.Setup.V1;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.KnownIdentities;
using ThunderbirdsBoardGameEngine.TestUtils.GameState;
using Xunit;

namespace ThunderbirdsBoardGameEngine.GameState.ComponentTests.MoveThunderbird
{
    public class MoveThunderbirdTests
    {
        [Fact]
        public async Task MovesThunderbirdAndPersistsWhenGatewayReturnsValidDecision()
        {
            // Arrange
            var game = CreateValidGame();
            var gameId = game.Id;

            var gateway = new FakeValidateMovementGateway(new ValidateMovementDecision(true, Array.Empty<string>()));

            await using var testHost = GameStateTestHost.CreateBuilder()
                .WithService<IValidateMovementGateway>(gateway)
                .Build();

            await testHost.Repository.CreateNewGameSession(game, TestContext.Current.CancellationToken);

            var command = new MoveThunderbirdCommand(
                gameId,
                KnownThunderbirdCodes.Thunderbird1,
                KnownLocationCodes.Europe
            );

            // Act
            var result = await testHost.Mediator.Send(command, TestContext.Current.CancellationToken);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(gameId, result.GameSession.Id);
            Assert.Equal(KnownLocationCodes.Europe, result.GameSession.GetThunderbirdMachineLocation(KnownThunderbirdCodes.Thunderbird1));

            AssertThunderbirdOneLocation(testHost.Repository, gameId, KnownLocationCodes.Europe);
        }

        [Fact]
        public async Task ThrowsThunderbirdMovementRejectedExceptionWhenGatewayReturnsInvalidDecision()
        {
            // Arrange
            var game = CreateValidGame();
            var gameId = game.Id;

            var gateway = new FakeValidateMovementGateway(new ValidateMovementDecision(false, new[] { "Movement not allowed" }));

            await using var testHost = GameStateTestHost.CreateBuilder()
                .WithService<IValidateMovementGateway>(gateway)
                .Build();

            await testHost.Repository.CreateNewGameSession(game, TestContext.Current.CancellationToken);

            var command = new MoveThunderbirdCommand(
                gameId,
                KnownThunderbirdCodes.Thunderbird1,
                KnownLocationCodes.Europe
            );

            // Act & Assert
            await Assert.ThrowsAsync<ThunderbirdMovementRejectedException>(() => testHost.Mediator.Send(command, TestContext.Current.CancellationToken));

            AssertThunderbirdOneLocation(testHost.Repository, gameId, KnownLocationCodes.SouthPacific);
        }

        private static Game CreateValidGame()
        {
            var gameFactory = new StandardGameSetupFactory();
            return gameFactory.Create(Guid.NewGuid(), DateTimeOffset.UtcNow);
        }

        private static void AssertThunderbirdOneLocation(InMemoryGameRepository repository, Guid gameId, LocationCode expectedLocation)
        {
            var result = repository.GetGameSessionById(gameId, TestContext.Current.CancellationToken).Result;

            Assert.NotNull(result);
            Assert.Equal(expectedLocation, result.GetThunderbirdMachineLocation(KnownThunderbirdCodes.Thunderbird1));
        }
    }
}
