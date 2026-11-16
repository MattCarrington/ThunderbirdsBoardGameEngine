using ThunderbirdsBoardGameEngine.GameState.Domain;
using ThunderbirdsBoardGameEngine.GameState.Infrastructure.Persistence.Records;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;

namespace ThunderbirdsBoardGameEngine.GameState.Infrastructure.Persistence
{
    internal class GameRecordMapper
    {
        public GameRecord MapToGameRecord(Game game)
        {
            var characterStates = game.Characters
                .Select(cs =>
                    new CharacterStateRecord { GameId = game.Id, CharacterCode = cs.Key.Value, ThunderbirdCode = cs.Value.Value })
                .ToList();

            var thunderbirdMachineStates = game.Machines
                .Select(ms =>
                    new ThunderbirdMachineStateRecord { GameId = game.Id, ThunderbirdCode = ms.Key.Value, LocationCode = ms.Value.Value })
                .ToList();

            var turnOrderEntries = game.TurnOrder
                .Select((characterCode, index) =>
                    new GameTurnOrderEntryRecord { GameId = game.Id, Position = index, CharacterCode = characterCode.Value })
                .ToList();

            var gameRecord = new GameRecord
            {
                Id = game.Id,
                CreatedAtUtc = game.CreatedAtUtc,
                SetupVersion = game.SetupVersion,
                StateVersion = game.StateVersion,
                CharacterStates = characterStates,
                ThunderbirdMachineStates = thunderbirdMachineStates,
                ActiveTurnIndex = game.ActiveTurnIndex,
                TurnOrderEntries = turnOrderEntries
            };

            return gameRecord;
        }

        public Game MapToGame(GameRecord gameRecord)
        {
            var characters = gameRecord.CharacterStates
                .ToDictionary(
                    cs => new CharacterCode(cs.CharacterCode),
                    cs => new ThunderbirdCode(cs.ThunderbirdCode));

            var machines = gameRecord.ThunderbirdMachineStates
                .ToDictionary(
                    ms => new ThunderbirdCode(ms.ThunderbirdCode),
                    ms => new LocationCode(ms.LocationCode));

            var turnOrder = gameRecord.TurnOrderEntries
                .OrderBy(entry => entry.Position)
                .Select(entry => new CharacterCode(entry.CharacterCode))
                .ToList();

            var game = Game.Restore(
                id: gameRecord.Id,
                createdAtUtc: gameRecord.CreatedAtUtc,
                setupVersion: gameRecord.SetupVersion,
                stateVersion: gameRecord.StateVersion,
                machines: machines,
                characters: characters,
                turnOrder: turnOrder,
                activeTurnIndex: gameRecord.ActiveTurnIndex);

            return game;
        }
    }
}
