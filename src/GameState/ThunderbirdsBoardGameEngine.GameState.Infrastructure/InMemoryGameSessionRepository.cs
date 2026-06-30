using System.Collections.Concurrent;
using ThunderbirdsBoardGameEngine.GameState.Application.CreateGame;
using ThunderbirdsBoardGameEngine.GameState.Domain;

namespace ThunderbirdsBoardGameEngine.GameState.Infrastructure
{
    internal class InMemoryGameSessionRepository : IGameSessionRespository
    {
        private readonly ConcurrentDictionary<Guid, GameSession> _sessions = new();

        public Task SaveGameSession(GameSession gameSession)
        {
            // Save the game session to an in-memory store (for demonstration purposes)
            // In a real application, you would persist this to a database or other storage
            _sessions[gameSession.GameId] = gameSession;
            return Task.CompletedTask;
        }

        public Task<GameSession> GetGameSession(Guid gameId)
        {
            if (!_sessions.TryGetValue(gameId, out var gameSession))
            {
                throw new Exception("Game session not found.");
            }

            return Task.FromResult(gameSession);
        }
    }
}
