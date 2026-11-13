using ThunderbirdsBoardGameEngine.GameState.Domain.Setup.V2;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.KnownIdentities;
using Xunit;

namespace ThunderbirdsBoardGameEngine.GameState.Domain.UnitTests
{
    public class StandardGameSetupFactoryTests
    {
        [Fact]
        public void Create_WhenJohnNotOneOfSelectedCharacters_ShouldReturnGameWithExpectedInitialState()
        {
            // Arrange
            var factory = new StandardGameSetupFactory();

            var gameId = Guid.NewGuid();

            var creationTime = DateTimeOffset.UtcNow;

            var turnOrder = new List<CharacterCode>
            {
                KnownCharacterCodes.Scott,
                KnownCharacterCodes.Virgil,
                KnownCharacterCodes.Alan,
                KnownCharacterCodes.Gordon
            };

            // Act
            var game = factory.Create(gameId, creationTime, turnOrder);

            // Assert
            Assert.NotNull(game);
            Assert.Equal(gameId, game.Id);
            Assert.Equal(creationTime, game.CreatedAtUtc);
            Assert.NotEmpty(game.Characters);
            Assert.NotEmpty(game.Machines);
            Assert.NotEmpty(game.TurnOrder);

            Assert.Equal(0, game.StateVersion);
            Assert.Equal(6, game.Machines.Count);
            Assert.Equal(6, game.Characters.Count);
            Assert.Equal(turnOrder.Count, game.TurnOrder.Count);

            Assert.Equal(KnownLocationCodes.SouthPacific, game.Machines[KnownThunderbirdCodes.Thunderbird1]);
            Assert.Equal(KnownLocationCodes.SouthPacific, game.Machines[KnownThunderbirdCodes.Thunderbird2]);
            Assert.Equal(KnownLocationCodes.SouthPacific, game.Machines[KnownThunderbirdCodes.Thunderbird3]);
            Assert.Equal(KnownLocationCodes.SouthPacific, game.Machines[KnownThunderbirdCodes.Thunderbird4]);
            Assert.Equal(KnownLocationCodes.GeoStationaryOrbit, game.Machines[KnownThunderbirdCodes.Thunderbird5]);
            Assert.Equal(KnownLocationCodes.Europe, game.Machines[KnownThunderbirdCodes.Fab1]);

            Assert.Equal(KnownThunderbirdCodes.Thunderbird1, game.Characters[KnownCharacterCodes.Scott]);
            Assert.Equal(KnownThunderbirdCodes.Thunderbird2, game.Characters[KnownCharacterCodes.Virgil]);
            Assert.Equal(KnownThunderbirdCodes.Thunderbird3, game.Characters[KnownCharacterCodes.Alan]);
            Assert.Equal(KnownThunderbirdCodes.Thunderbird4, game.Characters[KnownCharacterCodes.Gordon]);
            Assert.Equal(KnownThunderbirdCodes.Thunderbird5, game.Characters[KnownCharacterCodes.John]);
            Assert.Equal(KnownThunderbirdCodes.Fab1, game.Characters[KnownCharacterCodes.LadyPenelope]);

            Assert.Equal<CharacterCode>(turnOrder, game.TurnOrder);
        }

        [Fact]
        public void Create_WhenJohnIsOneOfSelectedCharacters_ShouldReturnGameWithExpectedInitialState()
        {
            // Arrange
            var factory = new StandardGameSetupFactory();

            var gameId = Guid.NewGuid();

            var creationTime = DateTimeOffset.UtcNow;

            var turnOrder = new List<CharacterCode>
            {
                KnownCharacterCodes.Scott,
                KnownCharacterCodes.Virgil,
                KnownCharacterCodes.John,
                KnownCharacterCodes.Gordon
            };
            // Act
            var game = factory.Create(gameId, creationTime, turnOrder);

            // Assert
            Assert.NotNull(game);
            Assert.Equal(KnownLocationCodes.GeoStationaryOrbit, game.Machines[KnownThunderbirdCodes.Thunderbird3]);
        }
    }
}
