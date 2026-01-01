using ThunderbirdsBoardGameEngine.Catalog.Client.Interfaces.V1;
using ThunderbirdsBoardGameEngine.Catalog.Client.Internal.Routing.V1;
using ThunderbirdsBoardGameEngine.Catalog.Contracts.Dtos.V1;
using ThunderbirdsBoardGameEngine.Client.Infrastructure;

namespace ThunderbirdsBoardGameEngine.Catalog.Client.Clients.V1
{
    /// <summary>
    /// Default implementation of <see cref="IDisasterCardsClient"/> using <see cref="HttpClient"/>.
    /// </summary>
    /// <remarks>
    /// The HTTP pipeline is configured via DI; do not construct directly. 
    /// Versioning is applied by a delegating handler that sets the <c>X-Api-Version</c> header.
    /// </remarks>
    public sealed class DisasterCardsClient : ApiClientBase, IDisasterCardsClient
    {

        /// <summary>
        /// Creates the client. Prefer resolving via DI to ensure the HTTP pipeline is configured.
        /// </summary>
        /// <param name="httpClient">Configured <see cref="HttpClient"/> supplied by DI.</param>
        public DisasterCardsClient(HttpClient httpClient) : base(httpClient)
        {            
        }

        /// <inheritdoc />
        public async Task<ApiResult<IReadOnlyList<DisasterCardDto>>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            using var response = await _httpClient.GetAsync(ApiRoutes.DisasterCards, cancellationToken);
            return await HandleResponse<IReadOnlyList<DisasterCardDto>>(response, cancellationToken);
        }
    }
}
