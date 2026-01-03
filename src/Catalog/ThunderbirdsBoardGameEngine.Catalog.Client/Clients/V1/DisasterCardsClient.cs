using ThunderbirdsBoardGameEngine.Catalog.Client.Interfaces.V1;
using ThunderbirdsBoardGameEngine.Catalog.Client.Internal.Routing.V1;
using ThunderbirdsBoardGameEngine.Catalog.Contracts.Dtos.V1;
using ThunderbirdsBoardGameEngine.Client.Infrastructure;
using ThunderbirdsBoardGameEngine.Client.Infrastructure.Handlers;

namespace ThunderbirdsBoardGameEngine.Catalog.Client.Clients.V1
{
    /// <summary>
    /// Default implementation of <see cref="IDisasterCardsClient"/> using <see cref="HttpClient"/>.
    /// </summary>
    /// <remarks>
    /// The HTTP pipeline is configured via DI; do not construct directly. 
    /// Versioning is applied by a delegating handler that sets the <c>X-Api-Version</c> header.
    /// </remarks>
    public sealed class DisasterCardsClient : IDisasterCardsClient
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpResponseHandler _httpResponseHandler;

        /// <summary>
        /// Creates the client. Prefer resolving via DI to ensure the HTTP pipeline is configured.
        /// </summary>
        /// <param name="httpClient">Configured <see cref="HttpClient"/> supplied by DI.</param>
        /// <param name="httpResponseHandler">Configure <see cref="IHttpResponseHandler"/> supplied by DI.</param>
        public DisasterCardsClient(HttpClient httpClient, IHttpResponseHandler httpResponseHandler)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _httpResponseHandler = httpResponseHandler ?? throw new ArgumentNullException(nameof(httpResponseHandler));
        }

        /// <inheritdoc />
        public async Task<ApiResult<IReadOnlyList<DisasterCardDto>>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            using var response = await _httpClient.GetAsync(ApiRoutes.DisasterCards, cancellationToken);
            return await _httpResponseHandler.HandleResponseAsync<IReadOnlyList<DisasterCardDto>>(response, cancellationToken);
        }
    }
}
