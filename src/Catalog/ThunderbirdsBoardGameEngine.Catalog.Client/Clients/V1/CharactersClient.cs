using ThunderbirdsBoardGameEngine.Catalog.Client.Interfaces.V1;
using ThunderbirdsBoardGameEngine.Catalog.Client.Internal.Routing.V1;
using ThunderbirdsBoardGameEngine.Catalog.Contracts.Dtos.V1;
using ThunderbirdsBoardGameEngine.Client.Infrastructure;
using ThunderbirdsBoardGameEngine.Client.Infrastructure.Interfaces;

namespace ThunderbirdsBoardGameEngine.Catalog.Client.Clients.V1
{
    [Obsolete("Catalog API is deprecated. Use Reference Data instead")]
    internal class CharactersClient : ICharactersClient
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpResponseHandler _httpResponseHandler;

        public CharactersClient(HttpClient httpClient, IHttpResponseHandler httpResponseHandler)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _httpResponseHandler = httpResponseHandler ?? throw new ArgumentNullException(nameof(httpResponseHandler));
        }

        public async Task<ApiResult<IReadOnlyList<CharacterDto>>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, ApiRoutes.Characters);

            using var response = await _httpClient.SendAsync(request, cancellationToken);

            return await _httpResponseHandler.HandleResponseAsync<IReadOnlyList<CharacterDto>>(response, cancellationToken);
        }
    }
}
