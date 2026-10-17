using ThunderbirdsBoardGameEngine.Client.Core;
using ThunderbirdsBoardGameEngine.GameState.Contracts.Dtos.V1;

namespace ThunderbirdsBoardGameEngine.Api.UserJourneys.Support
{
    public sealed class GameJourneyContext
    {
        public Guid GameId { get; set; }

        public GameStateResponseDto? CurrentGame { get; set; }

        public IDictionary<string, string> ThunderbirdMachineLocations { get; set; } = new Dictionary<string, string>();

        public ApiResult<GameStateResponseDto>? LastApiResult { get; set; }
    }
}