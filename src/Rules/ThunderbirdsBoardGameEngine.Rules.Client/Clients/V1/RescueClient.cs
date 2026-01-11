using System.Text;
using System.Text.Json;
using ThunderbirdsBoardGameEngine.Client.Infrastructure;
using ThunderbirdsBoardGameEngine.Client.Infrastructure.Interfaces;
using ThunderbirdsBoardGameEngine.Rules.Client.Interfaces.V1;
using ThunderbirdsBoardGameEngine.Rules.Client.Routing.V1;
using ThunderbirdsBoardGameEngine.Rules.Contracts.Dtos.Rescue.CalculateRescueTarget.V1;

namespace ThunderbirdsBoardGameEngine.Rules.Client.Clients.V1
{
    internal sealed class RescueClient : IRescueClient
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpResponseHandler _httpResponseHandler;

        public RescueClient(HttpClient httpClient, IHttpResponseHandler httpResponseHandler)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _httpResponseHandler = httpResponseHandler ?? throw new ArgumentNullException(nameof(httpResponseHandler));
        }

        public async Task<ApiResult<CalculateRescueTargetResponseDto>> CalculateRescueTargetAsync(
            string disasterCardCode,
            CalculateRescueTargetRequestDto request,
            CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(disasterCardCode);
            ArgumentNullException.ThrowIfNull(request);

            var encodedCode = Uri.EscapeDataString(disasterCardCode);

            var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

            var route = ApiRoutes.RescueTarget.Replace("{disasterCardCode}", encodedCode);

            using var message = new HttpRequestMessage(HttpMethod.Post, route)
            {
                Content = content
            };

            using var response = await _httpClient.SendAsync(message, cancellationToken);

            return await _httpResponseHandler.HandleResponseAsync<CalculateRescueTargetResponseDto>(response, cancellationToken);
        }
    }
}
