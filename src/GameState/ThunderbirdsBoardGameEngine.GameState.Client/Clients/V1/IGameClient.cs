using ThunderbirdsBoardGameEngine.Client.Core;
using ThunderbirdsBoardGameEngine.GameState.Contracts.Dtos.V1;
using ThunderbirdsBoardGameEngine.GameState.Contracts.Dtos.V1.ThunderbirdMachines;

namespace ThunderbirdsBoardGameEngine.GameState.Client.Clients.V1
{
    public interface IGameClient
    {
        Task<ApiResult<GameStateResponseDto>> CreateGameAsync(CancellationToken cancellationToken = default);

        Task<ApiResult<GameStateResponseDto>> GetGameStateAsync(Guid gameId, CancellationToken cancellationToken = default);

        Task<ApiResult<GameStateResponseDto>> MoveThunderbirdMachineAsync(
            Guid gameId,
            string thunderbirdCode,
            MoveThunderbirdMachineRequestDto request,
            CancellationToken cancellationToken = default);
    }
}