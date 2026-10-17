using ThunderbirdsBoardGameEngine.Client.Core;
using ThunderbirdsBoardGameEngine.GameState.Contracts.Dtos.V1;
using ThunderbirdsBoardGameEngine.GameState.Contracts.Dtos.V1.ThunderbirdMachines;

namespace ThunderbirdsBoardGameEngine.GameState.Client.Interfaces.V1
{
    /// <summary>
    /// Provides asynchronous operations to create a game, retrieve a game's state, and move a Thunderbird machine.
    /// </summary>
    public interface IGameClient
    {
        /// <summary>
        /// Creates a new game and returns the initial game state.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
        /// <returns>A task that yields an ApiResult containing the created game's state.</returns>
        Task<ApiResult<GameStateResponseDto>> CreateGameAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves the current state of the specified game.
        /// </summary>
        /// <param name="gameId">Unique identifier of the game to retrieve.</param>
        /// <param name="cancellationToken">Optional cancellation token to cancel the asynchronous operation.</param>
        /// <returns>A task that yields an ApiResult containing the game's state as a GameStateResponseDto.</returns>
        Task<ApiResult<GameStateResponseDto>> GetGameStateAsync(Guid gameId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Moves a Thunderbird machine to a new position and returns the updated game state.
        /// </summary>
        /// <param name="gameId">Identifier of the game containing the Thunderbird machine.</param>
        /// <param name="thunderbirdCode">Code that uniquely identifies the Thunderbird machine to move.</param>
        /// <param name="request">Request specifying the target position and any movement options.</param>
        /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
        /// <returns>An ApiResult containing a GameStateResponseDto representing the game state after the move.</returns>
        Task<ApiResult<GameStateResponseDto>> MoveThunderbirdMachineAsync(
            Guid gameId,
            string thunderbirdCode,
            MoveThunderbirdMachineRequestDto request,
            CancellationToken cancellationToken = default);
    }
}