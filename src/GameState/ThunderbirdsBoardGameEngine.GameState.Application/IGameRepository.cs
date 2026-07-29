using ThunderbirdsBoardGameEngine.GameState.Domain;

namespace ThunderbirdsBoardGameEngine.GameState.Application
{
    public interface IGameRepository
    {
        Task CreateNewGameSession(Game game, CancellationToken cancellationToken);

        Task<Game> GetGameSessionById(Guid gameId, CancellationToken cancellationToken);

        Task UpdateGameSession(Game game, CancellationToken cancellationToken);
    }
}
