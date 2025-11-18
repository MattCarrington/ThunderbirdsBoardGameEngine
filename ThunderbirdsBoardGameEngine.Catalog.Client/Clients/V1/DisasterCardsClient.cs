using System.Text.Json;
using ThunderbirdsBoardGameEngine.Catalog.Client.Interfaces.V1;
using ThunderbirdsBoardGameEngine.Catalog.Client.Internal.Routing.V1;
using ThunderbirdsBoardGameEngine.Catalog.Client.Internal.Serialization;
using ThunderbirdsBoardGameEngine.Catalog.Contracts.Dtos.V1;

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

        /// <summary>
        /// Creates the client. Prefer resolving via DI to ensure the HTTP pipeline is configured.
        /// </summary>
        /// <param name="httpClient">Configured <see cref="HttpClient"/> supplied by DI.</param>
        public DisasterCardsClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <inheritdoc />
        public async Task<ApiResult<IReadOnlyList<DisasterCardDto>>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            using var response = await _httpClient.GetAsync(ApiRoutes.DisasterCards, cancellationToken);
            return await HandleResponse<IReadOnlyList<DisasterCardDto>>(response, cancellationToken);
        }
        
        private static async Task<ApiResult<T>> HandleResponse<T>(HttpResponseMessage response, CancellationToken cancellationToken)
        {
            try
            {
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync(cancellationToken);
                    var data = JsonSerializer.Deserialize<T>(content, JsonDefaults.CamelCase);

                    return data == null
                        ? ApiResult<T>.Failure("Deserialized content was null.", response.StatusCode)
                        : ApiResult<T>.SuccessResult(data, response.StatusCode);
                }

                var errorMessage = await response.Content.ReadAsStringAsync(cancellationToken);
                return ApiResult<T>.Failure(errorMessage, response.StatusCode);
            }
            catch (OperationCanceledException)
            {
                throw; // Preserve cancellation
            }
            catch (JsonException ex)
            {
                return ApiResult<T>.Failure($"Deserialization error: {ex.Message}", response.StatusCode);
            }           
            catch (Exception ex)
            {
                return ApiResult<T>.Failure($"Unexpected error: {ex.Message}", response.StatusCode);
            }
        }
    }
}
