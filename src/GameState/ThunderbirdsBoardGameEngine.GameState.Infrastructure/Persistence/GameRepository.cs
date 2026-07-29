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
            var gameRecord = await _dbContext.Games.FirstOrDefaultAsync(g => g.Id == gameId, cancellationToken)
                ?? throw new InvalidOperationException($"Game with ID {gameId} not found.");

            var game = _mapper.MapToGame(gameRecord);

            return game;
        }
    }
}
