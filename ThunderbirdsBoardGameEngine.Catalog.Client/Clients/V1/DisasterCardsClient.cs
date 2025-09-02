using System.Text.Json;
using ThunderbirdsBoardGameEngine.Catalog.Client.Exceptions;
using ThunderbirdsBoardGameEngine.Catalog.Client.Interfaces.V1;
using ThunderbirdsBoardGameEngine.Catalog.Client.Internal.Routing.V1;
using ThunderbirdsBoardGameEngine.Catalog.Client.Internal.Serialization;
using ThunderbirdsBoardGameEngine.Catalog.Contracts.Dtos.V1;

namespace ThunderbirdsBoardGameEngine.Catalog.Client.Clients.V1
{
    public class DisasterCardsClient : IDisasterCardsClient
    {
        private readonly HttpClient _httpClient;

        public DisasterCardsClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ApiResult<IReadOnlyList<DisasterCardDto>>> GetAllAsync()
        {
            var response = await _httpClient.GetAsync(ApiRoutes.DisasterCards);
            return await HandleResponse<IReadOnlyList<DisasterCardDto>>(response);
        }

        private async Task<ApiResult<T>> HandleResponse<T>(HttpResponseMessage response)
        {
            try
            {
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var data = JsonSerializer.Deserialize<T>(content, JsonDefaults.CamelCase);

                    return data == null
                        ? throw new ApiDeserializationException("Deserialized content was null.")
                        : ApiResult<T>.SuccessResult(data, response.StatusCode);
                }

                var errorMessage = await response.Content.ReadAsStringAsync();
                return ApiResult<T>.Failure(errorMessage, response.StatusCode);
            }
            catch (JsonException ex)
            {
                return ApiResult<T>.Failure($"Deserialization error: {ex.Message}", response.StatusCode);
            }
            catch (ApiClientException ex)
            {
                return ApiResult<T>.Failure(ex.Message, response.StatusCode);
            }
            catch (Exception ex)
            {
                return ApiResult<T>.Failure($"Unexpected error: {ex.Message}", response.StatusCode);
            }
        }
    }
}
