using Microsoft.EntityFrameworkCore;
using ThunderbirdsBoardGameEngine.GameState.Application;
using ThunderbirdsBoardGameEngine.GameState.Domain;
using ThunderbirdsBoardGameEngine.GameState.Infrastructure.Persistence.Records;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;

namespace ThunderbirdsBoardGameEngine.GameState.Infrastructure.Persistence
{
    internal class GameRepository : IGameRepository
    {
        private readonly GameStateDbContext _dbContext;
        private readonly GameRecordMapper _mapper;

        public GameRepository(GameStateDbContext dbContext, GameRecordMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task CreateNewGameSession(Game game, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(game, nameof(game));

            var gameRecord = _mapper.MapToGameRecord(game);

            await _dbContext.AddAsync(gameRecord, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<Game> GetGameSessionById(Guid gameId, CancellationToken cancellationToken)
        {
            var game = await _dbContext.Games
                .AsNoTracking()
                .AsSplitQuery()
                .Include(game => game.CharacterStates)
                .Include(game => game.ThunderbirdMachineStates)
                .SingleOrDefaultAsync(
                    game => game.Id == gameId,
                    cancellationToken);

            return game is null ? null : _mapper.MapToGame(game);
        }

        public async Task UpdateGameSession(Game game, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(game, nameof(game));

            var persistedGame = await _dbContext.Games
               .AsSplitQuery()
               .Include(record => record.CharacterStates)
               .Include(record => record.ThunderbirdMachineStates)
               .SingleOrDefaultAsync(
                   record => record.Id == game.Id,
                   cancellationToken)
               ?? throw new DbUpdateConcurrencyException(
                   $"Game {game.Id} no longer exists.");

            UpdateMachineStates(game, persistedGame);
            UpdateCharacterStates(game, persistedGame);

            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        private static void UpdateMachineStates(Game game, GameRecord persistedGame)
        {
            if (persistedGame.ThunderbirdMachineStates.Count != game.Machines.Count)
            {
                throw new InvalidOperationException(
                    "Persisted machine roster does not match the game.");
            }

            foreach (var persistedMachine in persistedGame.ThunderbirdMachineStates)
            {
                var machineCode = new ThunderbirdCode(persistedMachine.ThunderbirdCode);

                if (!game.Machines.TryGetValue(machineCode, out var location))
                {
                    throw new InvalidOperationException(
                        $"Machine {machineCode.Value} is missing from the game.");
                }

                persistedMachine.LocationCode = location.Value;
            }
        }

        private static void UpdateCharacterStates(Game game, GameRecord persistedGame)
        {
            if (persistedGame.CharacterStates.Count != game.Characters.Count)
            {
                throw new InvalidOperationException(
                    "Persisted character roster does not match the game.");
            }

            foreach (var persistedCharacter in persistedGame.CharacterStates)
            {
                var characterCode = new CharacterCode(persistedCharacter.CharacterCode);

                if (!game.Characters.TryGetValue(characterCode, out var thunderbird))
                {
                    throw new InvalidOperationException(
                        $"Character {characterCode.Value} is missing from the game.");
                }

                persistedCharacter.ThunderbirdCode = thunderbird.Value;
            }
        }
    }
}
