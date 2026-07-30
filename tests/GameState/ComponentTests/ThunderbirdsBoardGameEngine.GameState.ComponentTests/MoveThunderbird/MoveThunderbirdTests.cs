using MediatR;
using Microsoft.Extensions.DependencyInjection;
using ThunderbirdsBoardGameEngine.GameState.Application;
using ThunderbirdsBoardGameEngine.GameState.Application.Exceptions;
using ThunderbirdsBoardGameEngine.GameState.Application.MoveThunderbird;
using ThunderbirdsBoardGameEngine.GameState.Domain;
using ThunderbirdsBoardGameEngine.GameState.Domain.Setup.V1;
using ThunderbirdsBoardGameEngine.GameState.Infrastructure;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.KnownIdentities;
using ThunderbirdsBoardGameEngine.TestUtils.ReferenceData.Fixtures;
using Xunit;

namespace ThunderbirdsBoardGameEngine.GameState.ComponentTests.MoveThunderbird
{
    public class MoveThunderbirdTests
    {
        [Fact]
        public async Task MovesThunderbirdAndPersistsWhenGatewayReturnsValidDecision()
        {
            // Arrange
            var repository = new InMemoryGameRepository();

            var game = CreateValidGame();
            var gameId = game.Id;

            await repository.CreateNewGameSession(game, TestContext.Current.CancellationToken);

            var gateway = new FakeValidateMovementGateway(new ValidateMovementDecision(true, Array.Empty<string>()));

            var mediator = CreateMediator(repository, gateway);

            var command = new MoveThunderbirdCommand(
                gameId,
                KnownThunderbirdCodes.Thunderbird1,
                KnownLocationCodes.Europe
            );

            // Act
            var result = await mediator.Send(command, TestContext.Current.CancellationToken);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(gameId, result.GameSession.Id);
            Assert.Equal(KnownLocationCodes.Europe, result.GameSession.GetThunderbirdMachineLocation(KnownThunderbirdCodes.Thunderbird1));

            AssertThunderbirdOneLocation(repository, gameId, KnownLocationCodes.Europe);
        }

        [Fact]
        public async Task ThrowsThunderbirdMovementRejectedExceptionWhenGatewayReturnsInvalidDecision()
        {
            // Arrange
            var repository = new InMemoryGameRepository();

            var game = CreateValidGame();

            var gameId = game.Id;

            await repository.CreateNewGameSession(game, TestContext.Current.CancellationToken);

            var gateway = new FakeValidateMovementGateway(new ValidateMovementDecision(false, new[] { "Invalid movement." }));

            var mediator = CreateMediator(repository, gateway);

            var command = new MoveThunderbirdCommand(
                gameId,
                KnownThunderbirdCodes.Thunderbird1,
                KnownLocationCodes.Europe
            );
            // Act & Assert
            await Assert.ThrowsAsync<ThunderbirdMovementRejectedException>(() => mediator.Send(command, TestContext.Current.CancellationToken));

            AssertThunderbirdOneLocation(repository, gameId, KnownLocationCodes.SouthPacific);
        }

        private static Game CreateValidGame()
        {
            var gameFactory = new StandardGameSetupFactory();
            return gameFactory.Create(Guid.NewGuid(), DateTimeOffset.UtcNow);
        }

        private static IMediator CreateMediator(IGameRepository repository, IValidateMovementGateway gateway)
        {
            var services = new ServiceCollection();
            services.AddGameState();
            services.AddGameStateIntegrity();
            services.AddSingleton(repository);
            services.AddFakeCatalogs();
            services.AddSingleton(gateway);
            var sp = services.BuildServiceProvider();
            return sp.GetRequiredService<IMediator>();
        }

        private static void AssertThunderbirdOneLocation(InMemoryGameRepository repository, Guid gameId, LocationCode expectedLocation)
        {
            var result = repository.GetGameSessionById(gameId, TestContext.Current.CancellationToken).Result;

            Assert.NotNull(result);
            Assert.Equal(expectedLocation, result.GetThunderbirdMachineLocation(KnownThunderbirdCodes.Thunderbird1));
        }
    }
}
