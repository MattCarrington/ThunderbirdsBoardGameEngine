using ThunderbirdsBoardGameEngine.GameState.Domain;

namespace ThunderbirdsBoardGameEngine.GameState.Application
{
    public interface IGameRepository
    {
        Task SaveGameSession(Game game, CancellationToken cancellationToken);
    }
}
