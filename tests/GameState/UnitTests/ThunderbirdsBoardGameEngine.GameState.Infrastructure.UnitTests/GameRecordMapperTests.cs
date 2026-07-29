using ThunderbirdsBoardGameEngine.GameState.Domain;
using ThunderbirdsBoardGameEngine.GameState.Infrastructure.Persistence;
using ThunderbirdsBoardGameEngine.GameState.Infrastructure.Persistence.Records;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
using Xunit;

namespace ThunderbirdsBoardGameEngine.GameState.Infrastructure.UnitTests
{
    public class GameRecordMapperTests
    {
        [Fact]
        public void MapToGameRecord_ShouldMapGameToGameRecord_WhenValidGame()
        {
            // Arrange
            var gameId = Guid.NewGuid();
            var createdAtUtc = DateTime.UtcNow;
            var setupVersion = "v1";

            var characters = new Dictionary<CharacterCode, ThunderbirdCode>
            {
                { new CharacterCode("CHAR1"), new ThunderbirdCode("TB1") },
                { new CharacterCode("CHAR2"), new ThunderbirdCode("TB2") }
            };

            var machines = new Dictionary<ThunderbirdCode, LocationCode>
            {
                { new ThunderbirdCode("TB1"), new LocationCode("LOC1") },
                { new ThunderbirdCode("TB2"), new LocationCode("LOC2") }
            };

            var game = Game.Create(gameId, createdAtUtc, setupVersion, machines, characters);

            var mapper = new GameRecordMapper();

            // Act
            var gameRecord = mapper.MapToGameRecord(game);

            // Assert
            Assert.Equal(gameId, gameRecord.Id);
            Assert.Equal(createdAtUtc, gameRecord.CreatedAtUtc);
            Assert.Equal(setupVersion, gameRecord.SetupVersion);
            Assert.Equal(characters.Count, gameRecord.CharacterStates.Count);
            Assert.Equal(machines.Count, gameRecord.ThunderbirdMachineStates.Count);
        }

        [Fact]
        public void MapToGame_ShouldMapGameRecordToGame_WhenValidGameRecord()
        {
            // Arrange
            var gameId = Guid.NewGuid();
            var createdAtUtc = DateTime.UtcNow;
            var setupVersion = "v1";

            var characterStates = new List<CharacterStateRecord>
            {
                new() { GameId = gameId, CharacterCode = "CHAR1", ThunderbirdCode = "TB1" },
                new() { GameId = gameId, CharacterCode = "CHAR2", ThunderbirdCode = "TB2" }
            };

            var machineStates = new List<ThunderbirdMachineStateRecord>
            {
                new() { GameId = gameId, ThunderbirdCode = "TB1", LocationCode = "LOC1" },
                new() { GameId = gameId, ThunderbirdCode = "TB2", LocationCode = "LOC2" }
            };

            var gameRecord = new GameRecord
            {
                Id = gameId,
                CreatedAtUtc = createdAtUtc,
                SetupVersion = setupVersion,
                CharacterStates = characterStates,
                ThunderbirdMachineStates = machineStates
            };

            var mapper = new GameRecordMapper();

            // Act
            var game = mapper.MapToGame(gameRecord);

            // Assert
            Assert.Equal(gameId, game.Id);
            Assert.Equal(createdAtUtc, game.CreatedAtUtc);
            Assert.Equal(setupVersion, game.SetupVersion);
            Assert.Equal(characterStates.Count, game.Characters.Count);
            Assert.Equal(machineStates.Count, game.Machines.Count);
        }
    }
}
