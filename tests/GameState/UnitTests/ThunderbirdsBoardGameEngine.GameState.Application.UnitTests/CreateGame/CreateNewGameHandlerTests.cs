using NSubstitute;
using ThunderbirdsBoardGameEngine.GameState.Application.CreateGame;
using ThunderbirdsBoardGameEngine.GameState.Domain;
using ThunderbirdsBoardGameEngine.GameState.Domain.Setup.V1;
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

            var handler = new CreateNewGameHandler(
                new StandardGameSetupFactory(),
                validator,
                repository);

            // Act
            var result = await handler.Handle(new CreateNewGameCommand(), CancellationToken.None);

            // Assert
            validator.Received(1)
                .Validate(Arg.Any<Game>());

            await repository.Received(1)
                .CreateNewGameSession(
                    Arg.Any<Game>(),
                    Arg.Any<CancellationToken>());

            Assert.NotNull(result);
            Assert.NotNull(result.GameSession);
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
        public async Task Handle_ShouldNotPersistGame_WhenIntegrityValidationFails()
        {
            // Arrange
            var validator = Substitute.For<IGameStateIntegrityValidator>();
            var repository = Substitute.For<IGameRepository>();

            validator
                .When(x => x.Validate(Arg.Any<Game>()))
                .Do(_ => throw new InvalidOperationException("Invalid game state."));

            var handler = new CreateNewGameHandler(
                new StandardGameSetupFactory(),
                validator,
                repository);

            // Act
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => handler.Handle(new CreateNewGameCommand(), CancellationToken.None));

            // Assert
            validator.Received(1)
                .Validate(Arg.Any<Game>());

            await repository.DidNotReceive()
                .CreateNewGameSession(
                    Arg.Any<Game>(),
                    Arg.Any<CancellationToken>());
        }
    }
}
