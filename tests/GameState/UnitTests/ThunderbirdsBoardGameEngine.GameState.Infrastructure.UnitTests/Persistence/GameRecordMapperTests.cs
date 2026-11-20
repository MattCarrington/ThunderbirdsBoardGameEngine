using ThunderbirdsBoardGameEngine.GameState.Domain;
using ThunderbirdsBoardGameEngine.GameState.Infrastructure.Persistence;
using ThunderbirdsBoardGameEngine.GameState.Infrastructure.Persistence.Records;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
using Xunit;

namespace ThunderbirdsBoardGameEngine.GameState.Infrastructure.UnitTests.Persistence
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
            var stateVersion = 17;
            var activeTurnIndex = 1;

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

            var turnOrder = new List<CharacterCode> { new("CHAR1"), new("CHAR2") };

            var game = Game.Restore(gameId, createdAtUtc, setupVersion, stateVersion, machines, characters, turnOrder, activeTurnIndex);

            var mapper = new GameRecordMapper();

            // Act
            var gameRecord = mapper.MapToGameRecord(game);

            // Assert
            Assert.Equal(gameId, gameRecord.Id);
            Assert.Equal(createdAtUtc, gameRecord.CreatedAtUtc);
            Assert.Equal(setupVersion, gameRecord.SetupVersion);
            Assert.Equal(stateVersion, gameRecord.StateVersion);
            Assert.Equal(characters.Count, gameRecord.CharacterStates.Count);
            Assert.Equal(machines.Count, gameRecord.ThunderbirdMachineStates.Count);
            Assert.Equal(activeTurnIndex, gameRecord.ActiveTurnIndex);

            Assert.Collection(gameRecord.TurnOrderEntries
                .OrderBy(entry => entry.Position),
                    first =>
                    {
                        Assert.Equal(0, first.Position);
                        Assert.Equal("CHAR1", first.CharacterCode);
                    },
                    second =>
                    {
                        Assert.Equal(1, second.Position);
                        Assert.Equal("CHAR2", second.CharacterCode);
                    });
        }

        [Fact]
        public void MapToGame_ShouldMapGameRecordToGame_WhenValidGameRecord()
        {
            // Arrange
            var gameId = Guid.NewGuid();
            var createdAtUtc = DateTime.UtcNow;
            var setupVersion = "v1";
            var stateVersion = 57;
            var activeTurnIndex = 1;

            var characterStates = new List<CharacterStateRecord>
            {
                new() { GameId = gameId, CharacterCode = "character-1", ThunderbirdCode = "TB1" },
                new() { GameId = gameId, CharacterCode = "character-2", ThunderbirdCode = "TB2" },
                new() { GameId = gameId, CharacterCode = "character-3", ThunderbirdCode = "TB1" }
            };

            var machineStates = new List<ThunderbirdMachineStateRecord>
            {
                new() { GameId = gameId, ThunderbirdCode = "TB1", LocationCode = "LOC1" },
                new() { GameId = gameId, ThunderbirdCode = "TB2", LocationCode = "LOC2" }
            };

            var turnOrderEntries = new List<GameTurnOrderEntryRecord>
            {
                new() { GameId = gameId, Position = 1, CharacterCode = "character-1" },
                new() { GameId = gameId, Position = 2, CharacterCode = "character-3" },
                new() { GameId = gameId, Position = 0, CharacterCode = "character-2" }
            };

            var gameRecord = new GameRecord
            {
                Id = gameId,
                CreatedAtUtc = createdAtUtc,
                SetupVersion = setupVersion,
                StateVersion = stateVersion,
                CharacterStates = characterStates,
                ThunderbirdMachineStates = machineStates,
                TurnOrderEntries = turnOrderEntries,
                ActiveTurnIndex = activeTurnIndex
            };

            var mapper = new GameRecordMapper();

            // Act
            var game = mapper.MapToGame(gameRecord);

            // Assert
            Assert.Equal(gameId, game.Id);
            Assert.Equal(createdAtUtc, game.CreatedAtUtc);
            Assert.Equal(setupVersion, game.SetupVersion);
            Assert.Equal(stateVersion, game.StateVersion);
            Assert.Equal(characterStates.Count, game.Characters.Count);
            Assert.Equal(machineStates.Count, game.Machines.Count);
            Assert.Equal(activeTurnIndex, game.ActiveTurnIndex);
            Assert.Equal(["character-2", "character-1", "character-3"], game.TurnOrder.Select(code => code.Value));
        }
    }
}
