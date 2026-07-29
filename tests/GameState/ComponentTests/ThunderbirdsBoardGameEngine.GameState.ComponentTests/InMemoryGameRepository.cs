using ThunderbirdsBoardGameEngine.GameState.Application;
using ThunderbirdsBoardGameEngine.GameState.Domain;

namespace ThunderbirdsBoardGameEngine.GameState.ComponentTests
{
    public class InMemoryGameRepository : IGameRepository
    {
        private readonly Dictionary<Guid, Game> _games = new();

        public Task CreateNewGameSession(Game gameSession, CancellationToken cancellationToken)
        {
            _games[gameSession.Id] = gameSession;
            return Task.CompletedTask;
        }

        public Task<Game> GetGameSessionById(Guid gameId, CancellationToken cancellationToken)
        {
            _games.TryGetValue(gameId, out var game);

            if (game is null)
            {
                throw new Exception("game not found");
            }

            return Task.FromResult(game);
        }

        public Task UpdateGameSession(Game game, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
