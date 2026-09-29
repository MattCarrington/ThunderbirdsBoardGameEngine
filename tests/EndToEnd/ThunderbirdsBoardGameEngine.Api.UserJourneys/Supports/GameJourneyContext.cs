using ThunderbirdsBoardGameEngine.GameState.Contracts.Dtos.V1;

namespace ThunderbirdsBoardGameEngine.Api.UserJourneys.Supports
{
    public sealed class GameJourneyContext
    {
        public Guid GameId { get; set; }

        public GameStateResponseDto? CurrentGame { get; set; }
    }
}