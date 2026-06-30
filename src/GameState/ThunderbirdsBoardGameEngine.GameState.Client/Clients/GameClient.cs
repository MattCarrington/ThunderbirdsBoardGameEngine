using System.Net.Http.Json;
using ThunderbirdsBoardGameEngine.Client.Infrastructure;
using ThunderbirdsBoardGameEngine.Client.Infrastructure.Interfaces;
using ThunderbirdsBoardGameEngine.GameState.Contracts.V1;

namespace ThunderbirdsBoardGameEngine.GameState.Client.Clients
{
    internal class GameClient : IGameClient
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpResponseHandler _httpResponseHandler;

        public GameClient(HttpClient httpClient, IHttpResponseHandler httpResponseHandler)
        {
            _httpClient = httpClient;
            _httpResponseHandler = httpResponseHandler;
        }

        public async Task<ApiResult<GameSessionDto>> CreateGameStateAsync(CancellationToken cancellationToken = default)
        {
            using var message = new HttpRequestMessage(HttpMethod.Post, "api/games/game/create");

            using var response = await _httpClient.SendAsync(message, cancellationToken);

            return await _httpResponseHandler.HandleResponseAsync<GameSessionDto>(response, cancellationToken);
        }

        public async Task<ApiResult<GameSessionDto>> GetGameStateAsync(Guid gameId, CancellationToken cancellationToken = default)
        {
            var route = $"api/games/game/{gameId}";

            using var message = new HttpRequestMessage(HttpMethod.Get, route);

            using var response = await _httpClient.SendAsync(message, cancellationToken);

            return await _httpResponseHandler.HandleResponseAsync<GameSessionDto>(response, cancellationToken);
        }

        public async Task<ApiResult<GameSessionDto>> MoveThunderbirdAsync(Guid gameId, MoveThunderbirdLocationRequestDto request, CancellationToken cancellationToken = default)
        {
            var route = $"api/games/game/{gameId}/move";

            using var message = new HttpRequestMessage(HttpMethod.Post, route)
            {
                Content = JsonContent.Create(request)
            };

            using var response = await _httpClient.SendAsync(message, cancellationToken);

            return await _httpResponseHandler.HandleResponseAsync<GameSessionDto>(response, cancellationToken);
        }
    }
}
