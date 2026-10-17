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
            var game = Game.Create(
                id: Guid.NewGuid(),
                createdAtUtc: DateTimeOffset.UtcNow,
                setupVersion: "1.0",
                machines: new Dictionary<ThunderbirdCode, LocationCode>
                {
                    { new ThunderbirdCode("TB1"), new LocationCode("LOC1") },
                    { new ThunderbirdCode("TB2"), new LocationCode("LOC2") },
                    { new ThunderbirdCode("TB3"), new LocationCode("LOC2") }

                },
                characters: new Dictionary<CharacterCode, ThunderbirdCode>
                {
                    { new CharacterCode("CHAR1"), new ThunderbirdCode("TB1") },
                    { new CharacterCode("CHAR2"), new ThunderbirdCode("TB1") },
                    { new CharacterCode("CHAR3"), new ThunderbirdCode("TB2") }
                });

            // Act
            var dto = game.ToDto();

            // Assert
            Assert.Equal(game.Id, dto.GameId);
            Assert.Equal(3, dto.ThunderbirdMachines.Count);

            var tb1Dto = dto.ThunderbirdMachines.First(m => m.ThunderbirdCode == "TB1");
            Assert.Equal("LOC1", tb1Dto.LocationCode);
            Assert.Equal(new List<string> { "CHAR1", "CHAR2" }, tb1Dto.OccupantCharacterCodes);

            var tb2Dto = dto.ThunderbirdMachines.First(m => m.ThunderbirdCode == "TB2");
            Assert.Equal("LOC2", tb2Dto.LocationCode);
            Assert.Equal(new List<string> { "CHAR3" }, tb2Dto.OccupantCharacterCodes);

            var tb3Dto = dto.ThunderbirdMachines.First(m => m.ThunderbirdCode == "TB3");
            Assert.Equal("LOC2", tb3Dto.LocationCode);
            Assert.Empty(tb3Dto.OccupantCharacterCodes);
        }
    }
}
