using ThunderbirdsBoardGameEngine.GameState.Application.Exceptions;
using ThunderbirdsBoardGameEngine.GameState.Application.TransferCharacters;
using ThunderbirdsBoardGameEngine.GameState.ComponentTests.Helpers;
using ThunderbirdsBoardGameEngine.GameState.Domain;
using ThunderbirdsBoardGameEngine.GameState.Domain.Setup.V1;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.KnownIdentities;
using Xunit;

namespace ThunderbirdsBoardGameEngine.GameState.ComponentTests.TransferCharacters
{
    public class TransferCharactersTests
    {
        [Fact]
        public async Task TransfersAndPersistsValidSingleCharacterTransfer()
        {
            // Arrange
            var game = CreateValidGame();
            var gameId = game.Id;

            var gateway = new FakeCharacterTransferRulesGateway(CharacterTransferDecision.Permitted());

            await using var testHost = GameStateTestHost.CreateBuilder()
                .WithService<ICharacterTransferRulesGateway>(gateway)
                .Build();

            await testHost.Repository.CreateNewGameSession(game, TestContext.Current.CancellationToken);

            var command = new TransferCharactersCommand(
                gameId,
                [
                    new CharacterAssignment(KnownCharacterCodes.Scott, KnownThunderbirdCodes.Thunderbird3)
                ]
            );

            // Act
            var result = await testHost.Mediator.Send(command, TestContext.Current.CancellationToken);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(gameId, result.GameSession.Id);
            Assert.Equal(KnownThunderbirdCodes.Thunderbird3, result.GameSession.GetCharacterAssignment(KnownCharacterCodes.Scott));

            var updatedGame = await testHost.Repository.GetGameSessionById(gameId, TestContext.Current.CancellationToken)
                ?? throw new InvalidOperationException("Game session not found after transfer.");

            Assert.Empty(updatedGame.GetThunderbirdMachineOccupancy(KnownThunderbirdCodes.Thunderbird1));
            Assert.Equal(KnownThunderbirdCodes.Thunderbird3, updatedGame.GetCharacterAssignment(KnownCharacterCodes.Scott));
        }

        [Fact]
        public async Task TransfersAndPersistsValidReciprocalSwap()
        {
            // Arrange
            var game = CreateValidGame();
            var gameId = game.Id;

            var gateway = new FakeCharacterTransferRulesGateway(CharacterTransferDecision.Permitted());

            await using var testHost = GameStateTestHost.CreateBuilder()
                .WithService<ICharacterTransferRulesGateway>(gateway)
                .Build();

            await testHost.Repository.CreateNewGameSession(game, TestContext.Current.CancellationToken);

            var command = new TransferCharactersCommand(
                gameId,
                [
                    new CharacterAssignment(KnownCharacterCodes.Scott, KnownThunderbirdCodes.Thunderbird4),
                    new CharacterAssignment(KnownCharacterCodes.Gordon, KnownThunderbirdCodes.Thunderbird1)
                ]
            );

            // Act
            var result = await testHost.Mediator.Send(command, TestContext.Current.CancellationToken);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(gameId, result.GameSession.Id);

            Assert.Equal(KnownThunderbirdCodes.Thunderbird4, result.GameSession.GetCharacterAssignment(KnownCharacterCodes.Scott));
            Assert.Equal(KnownThunderbirdCodes.Thunderbird1, result.GameSession.GetCharacterAssignment(KnownCharacterCodes.Gordon));

            var updatedGame = await testHost.Repository.GetGameSessionById(gameId, TestContext.Current.CancellationToken)
                ?? throw new InvalidOperationException("Game session not found after transfer.");

            Assert.Equal(KnownThunderbirdCodes.Thunderbird4, updatedGame.GetCharacterAssignment(KnownCharacterCodes.Scott));
            Assert.Equal(KnownThunderbirdCodes.Thunderbird1, updatedGame.GetCharacterAssignment(KnownCharacterCodes.Gordon));
        }

        [Fact]
        public async Task ThrowsCharacterTransferRejectedExceptionWhenGatewayReturnsInvalidDecision()
        {
            // Arrange
            var game = CreateValidGame();
            var gameId = game.Id;

            var gateway = new FakeCharacterTransferRulesGateway(CharacterTransferDecision.Rejected(CharacterTransferRejectionReason.MachinesNotCoLocated));

            await using var testHost = GameStateTestHost.CreateBuilder()
                .WithService<ICharacterTransferRulesGateway>(gateway)
                .Build();

            await testHost.Repository.CreateNewGameSession(game, TestContext.Current.CancellationToken);

            var command = new TransferCharactersCommand(
                gameId,
                [
                    new CharacterAssignment(KnownCharacterCodes.Scott, KnownThunderbirdCodes.Thunderbird5)
                ]
            );

            // Act & Assert
            await Assert.ThrowsAsync<CharacterTransferRejectedException>(() => testHost.Mediator.Send(command, TestContext.Current.CancellationToken));

            var updatedGame = await testHost.Repository.GetGameSessionById(gameId, TestContext.Current.CancellationToken)
                ?? throw new InvalidOperationException("Game session not found after transfer.");

            Assert.Equal(KnownThunderbirdCodes.Thunderbird1, updatedGame.GetCharacterAssignment(KnownCharacterCodes.Scott));
        }

        private static Game CreateValidGame()
        {
            var gameFactory = new StandardGameSetupFactory();
            return gameFactory.Create(Guid.NewGuid(), DateTimeOffset.UtcNow);
        }
    }
}
