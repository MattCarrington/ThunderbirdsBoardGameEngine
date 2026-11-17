using ThunderbirdsBoardGameEngine.Api.Mappers.GameState.V1;
using ThunderbirdsBoardGameEngine.GameState.Domain;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Api.UnitTests.Mappers.GameState.V1
{
    public class GameStateMappingExtensionsTests
    {
        [Fact]
        public void ToDto_ShouldMapGameToGameStateResponseDto()
        {
            // Arrange
            var turnOrder = new List<CharacterCode>
            {
                new("CHAR1"),
                new("CHAR2"),
                new("CHAR3")
            };

            var game = Game.Restore(
                id: Guid.NewGuid(),
                createdAtUtc: DateTimeOffset.UtcNow,
                setupVersion: "1.0",
                machines: new Dictionary<ThunderbirdCode, LocationCode>
                {
                    { new("TB1"), new("LOC1") },
                    { new("TB2"), new("LOC2") },
                    { new("TB3"), new("LOC2") }

                },
                characters: new Dictionary<CharacterCode, ThunderbirdCode>
                {
                    { new("CHAR1"), new("TB1") },
                    { new("CHAR2"), new("TB1") },
                    { new("CHAR3"), new("TB2") }
                },
                turnOrder: turnOrder,
                activeTurnIndex: 1,
                stateVersion: 1724);

            // Act
            var dto = game.ToDto();

            // Assert
            Assert.Equal(game.Id, dto.GameId);
            Assert.Equal(3, dto.ThunderbirdMachines.Count);
            Assert.Equal(turnOrder.Select(c => c.Value), dto.TurnOrder);
            Assert.Equal(turnOrder[1].Value, dto.ActiveCharacterCode);

            var tb1Dto = dto.ThunderbirdMachines.First(m => m.ThunderbirdCode == "TB1");
            Assert.Equal("LOC1", tb1Dto.LocationCode);
            Assert.Equal(["CHAR1", "CHAR2"], tb1Dto.OccupantCharacterCodes);

            var tb2Dto = dto.ThunderbirdMachines.First(m => m.ThunderbirdCode == "TB2");
            Assert.Equal("LOC2", tb2Dto.LocationCode);
            Assert.Equal(["CHAR3"], tb2Dto.OccupantCharacterCodes);

            var tb3Dto = dto.ThunderbirdMachines.First(m => m.ThunderbirdCode == "TB3");
            Assert.Equal("LOC2", tb3Dto.LocationCode);
            Assert.Empty(tb3Dto.OccupantCharacterCodes);
        }
    }
}
