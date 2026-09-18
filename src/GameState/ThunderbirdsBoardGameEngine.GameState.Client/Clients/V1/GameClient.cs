using System.Text;
using System.Text.Json;
using ThunderbirdsBoardGameEngine.Client.Core;
using ThunderbirdsBoardGameEngine.Client.Core.Interfaces;
using ThunderbirdsBoardGameEngine.GameState.Contracts.Dtos.V1;
using ThunderbirdsBoardGameEngine.GameState.Contracts.Dtos.V1.ThunderbirdMachines;

namespace ThunderbirdsBoardGameEngine.GameState.Client.Clients.V1
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

        public async Task<ApiResult<GameStateResponseDto>> CreateGameAsync(CancellationToken cancellationToken = default)
        {
            var route = "api/games";

            using var message = new HttpRequestMessage(HttpMethod.Post, route);

            using var response = await _httpClient.SendAsync(message, cancellationToken);

            return await _httpResponseHandler.HandleResponseAsync<GameStateResponseDto>(response, cancellationToken);
        }

        public async Task<ApiResult<GameStateResponseDto>> GetGameStateAsync(Guid gameId, CancellationToken cancellationToken = default)
        {
            if (gameId == Guid.Empty)
            {
                throw new ArgumentException("Game ID cannot be empty.", nameof(gameId));
            }

            var route = $"api/games/{gameId}";

            using var message = new HttpRequestMessage(HttpMethod.Get, route);

            using var response = await _httpClient.SendAsync(message, cancellationToken);

            return await _httpResponseHandler.HandleResponseAsync<GameStateResponseDto>(response, cancellationToken);
        }

        public async Task<ApiResult<GameStateResponseDto>> MoveThunderbirdMachineAsync(
            Guid gameId,
            string thunderbirdCode,
            MoveThunderbirdMachineRequestDto request,
            CancellationToken cancellationToken = default)
        {
            if (gameId == Guid.Empty)
            {
                throw new ArgumentException("Game ID cannot be empty.", nameof(gameId));
            }

            if (string.IsNullOrWhiteSpace(thunderbirdCode))
            {
                throw new ArgumentException("Thunderbird code cannot be null or whitespace.", nameof(thunderbirdCode));
            }

            var route = $"api/games/{gameId}/thunderbird-machines/{Uri.EscapeDataString(thunderbirdCode)}/movements";

            var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

            using var message = new HttpRequestMessage(HttpMethod.Post, route)
            {
                Content = content
            };

            using var response = await _httpClient.SendAsync(message, cancellationToken);

            return await _httpResponseHandler.HandleResponseAsync<GameStateResponseDto>(response, cancellationToken);
        }
    }
}
