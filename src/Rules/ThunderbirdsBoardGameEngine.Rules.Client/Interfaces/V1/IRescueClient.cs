using ThunderbirdsBoardGameEngine.Client.Infrastructure;
using ThunderbirdsBoardGameEngine.Rules.Contracts.Dtos.Rescue.CalculateRescueTarget.V1;

namespace ThunderbirdsBoardGameEngine.Rules.Client.Interfaces.V1
{
    /// <summary>
    /// Defines methods for calculating rescue targets based on disaster card information.
    /// </summary>
    public interface IRescueClient
    {
        /// <summary>
        /// Asynchronously calculates the rescue target for a specified disaster card using the provided request
        /// parameters.
        /// </summary>
        /// <param name="disasterCardCode">The unique code identifying the disaster card for which to calculate the rescue target. Cannot be null or
        /// empty.</param>
        /// <param name="request">An object containing the parameters required to perform the rescue target calculation. Cannot be null.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an ApiResult with the calculated
        /// rescue target response data.</returns>
        Task<ApiResult<CalculateRescueTargetResponseDto>> CalculateRescueTargetAsync(
            string disasterCardCode,
            CalculateRescueTargetRequestDto request,
            CancellationToken cancellationToken = default);
    }
}