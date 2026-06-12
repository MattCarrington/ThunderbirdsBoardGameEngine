using System.Text;
using System.Text.Json;
using ThunderbirdsBoardGameEngine.Client.Infrastructure;
using ThunderbirdsBoardGameEngine.Client.Infrastructure.Interfaces;
using ThunderbirdsBoardGameEngine.Rules.Client.Interfaces.V1;
using ThunderbirdsBoardGameEngine.Rules.Client.Routing.V1;
using ThunderbirdsBoardGameEngine.Rules.Contracts.Dtos.Movement.ValidateMovement.V1;

namespace ThunderbirdsBoardGameEngine.Rules.Client.Clients.V1
{
    internal class MovementClient : IMovementClient
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpResponseHandler _httpResponseHandler;

        public MovementClient(HttpClient httpClient, IHttpResponseHandler httpResponseHandler)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _httpResponseHandler = httpResponseHandler ?? throw new ArgumentNullException(nameof(httpResponseHandler));
        }

        public async Task<ApiResult<ValidateMovementResponseDto>> ValidateMovementAsync(
            string thunderbirdCode,
            ValidateMovementRequestDto request,
            CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(thunderbirdCode);
            ArgumentNullException.ThrowIfNull(request);

            var encodedCode = Uri.EscapeDataString(thunderbirdCode);

            var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

            var route = ApiRoutes.ValidateMovement.Replace("{thunderbirdCode}", encodedCode);

            using var message = new HttpRequestMessage(HttpMethod.Post, route)
            {
                Content = content
            };

            using var response = await _httpClient.SendAsync(message, cancellationToken);

            return await _httpResponseHandler.HandleResponseAsync<ValidateMovementResponseDto>(response, cancellationToken);
        }
    }
}
