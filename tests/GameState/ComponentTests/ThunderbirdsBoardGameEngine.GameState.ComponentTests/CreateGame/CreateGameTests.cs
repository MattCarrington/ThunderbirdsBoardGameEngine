using NSubstitute;
using ThunderbirdsBoardGameEngine.GameState.Application;
using ThunderbirdsBoardGameEngine.GameState.Application.CreateGame;
using ThunderbirdsBoardGameEngine.GameState.Domain.Setup.V1;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.KnownIdentities;
using Xunit;

namespace ThunderbirdsBoardGameEngine.GameState.ComponentTests.CreateGame
{
    public class CreateGameTests
    {
        [Fact]
        public async Task CanCreateValidNewGameAsync()
        {
            // Arrange
            var factory = new StandardGameSetupFactory();

            var repository = Substitute.For<IGameRepository>();

            var createGameHandler = new CreateNewGameHandler(factory, repository);

            var command = new CreateNewGameCommand();

            // Act
            var result = await createGameHandler.Handle(command, CancellationToken.None);

            // Assert
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
    }
}
