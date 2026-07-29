using ThunderbirdsBoardGameEngine.GameState.Application;
using ThunderbirdsBoardGameEngine.GameState.Domain;

namespace ThunderbirdsBoardGameEngine.GameState.ComponentTests
{
    public class InMemoryGameRepository : IGameRepository
    {
        private readonly Dictionary<Guid, Game> _games = new();

        public Task<Game> GetGameSessionById(Guid gameId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task CreateNewGameSession(Game gameSession, CancellationToken cancellationToken)
        {
            _games[gameSession.Id] = gameSession;
            return Task.CompletedTask;
        }
    }
}
