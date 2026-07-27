using ThunderbirdsBoardGameEngine.GameState.Domain;
using ThunderbirdsBoardGameEngine.GameState.Infrastructure.Persistence.Records;

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

            var gameRecord = new GameRecord
            {
                Id = game.Id,
                CreatedAtUtc = game.CreatedAtUtc,
                SetupVersion = game.SetupVersion,
                CharacterStates = characterStates,
                ThunderbirdMachineStates = thunderbirdMachineStates
            };

            return gameRecord;
        }
    }
}
