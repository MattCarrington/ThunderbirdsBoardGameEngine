using ThunderbirdsBoardGameEngine.GameState.Domain;

namespace ThunderbirdsBoardGameEngine.GameState.Application.CreateGame
{
    public interface IGameSessionRespository
    {
        Task<GameSession> GetGameSession(Guid gameId);

        Task SaveGameSession(GameSession gameSession);
    }
}