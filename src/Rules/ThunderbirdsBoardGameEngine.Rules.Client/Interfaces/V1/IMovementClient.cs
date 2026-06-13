using ThunderbirdsBoardGameEngine.Client.Infrastructure;
using ThunderbirdsBoardGameEngine.Rules.Contracts.Dtos.Movement.AccessibleLocations.V1;
using ThunderbirdsBoardGameEngine.Rules.Contracts.Dtos.Movement.ValidateMovement.V1;

namespace ThunderbirdsBoardGameEngine.Rules.Client.Interfaces.V1
{
    /// <summary>
    /// Client for interacting with movement related rules endpoints.
    /// </summary>
    public interface IMovementClient
    {
        /// <summary>
        /// Validates a proposed movement action for a given Thunderbird, returning details about the validity and cost of the movement.
        /// </summary>
        /// <param name="thunderbirdCode">The unique code of the Thunderbird.</param>
        /// <param name="request">The request containing movement details.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the validation result.</returns>
        Task<ApiResult<ValidateMovementResponseDto>> ValidateMovementAsync(string thunderbirdCode, ValidateMovementRequestDto request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves the accessible locations for a given Thunderbird.
        /// </summary>
        /// <param name="thunderbirdCode">The unique code of the Thunderbird.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the accessible locations.</returns>
        Task<ApiResult<AccessibleLocationsResponseDto>> GetAccessibleLocationsAsync(string thunderbirdCode, CancellationToken cancellationToken = default);
    }
}