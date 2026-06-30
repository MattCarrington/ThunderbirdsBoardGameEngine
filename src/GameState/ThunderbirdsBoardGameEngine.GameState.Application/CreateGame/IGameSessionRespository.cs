using ThunderbirdsBoardGameEngine.GameState.Domain;

namespace ThunderbirdsBoardGameEngine.GameState.Application.CreateGame
{
    public interface IGameSessionRespository
    {
        Task<GameSession> GetGameSession(Guid gameId, CancellationToken cancellationToken);

        Task SaveGameSession(GameSession gameSession, CancellationToken cancellationToken);
    }
}