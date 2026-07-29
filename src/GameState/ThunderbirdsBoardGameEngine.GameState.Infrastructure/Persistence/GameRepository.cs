using Microsoft.EntityFrameworkCore;
using ThunderbirdsBoardGameEngine.GameState.Application;
using ThunderbirdsBoardGameEngine.GameState.Domain;

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
    }
}
