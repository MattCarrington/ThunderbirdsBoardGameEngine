using ThunderbirdsBoardGameEngine.Catalog.Contracts.Dtos.V1;
using ThunderbirdsBoardGameEngine.Client.Infrastructure;

namespace ThunderbirdsBoardGameEngine.Catalog.Client.Interfaces.V1
{
    /// <summary>
    /// Client for retrieving disaster cards from the Catalog service (API v1).
    /// </summary>
    /// <remarks>
    /// This client is versioned via the HTTP pipeline and sends an <c>X-Api-Version</c> header.
    /// Register with <see cref="Extensions.ServiceCollectionExtensions.AddCatalogClients"/>. 
    /// </remarks>
    public interface IDisasterCardsClient
    {
        /// <summary>
        /// Retrieves all disaster cards.
        /// </summary>
        /// <param name="cancellationToken">A token to cancel the request.</param>
        /// <returns>
        /// An <see cref="ApiResult{T}"/> containing the set of cards on success; otherwise a failure result with status and message.
        /// </returns>
        Task<ApiResult<IReadOnlyList<DisasterCardDto>>> GetAllAsync(CancellationToken cancellationToken = default);
    }
}
