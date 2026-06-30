using ThunderbirdsBoardGameEngine.Client.Infrastructure;
using ThunderbirdsBoardGameEngine.GameState.Contracts.V1;

namespace ThunderbirdsBoardGameEngine.GameState.Client.Clients
{
    public interface IGameClient
    {
        Task<ApiResult<GameSessionDto>> CreateGameStateAsync(CancellationToken cancellationToken = default);

        Task<ApiResult<GameSessionDto>> GetGameStateAsync(Guid gameId, CancellationToken cancellationToken = default);

        Task<ApiResult<GameSessionDto>> MoveThunderbirdAsync(Guid gameId, MoveThunderbirdLocationRequestDto request, CancellationToken cancellationToken = default);
    }
}
