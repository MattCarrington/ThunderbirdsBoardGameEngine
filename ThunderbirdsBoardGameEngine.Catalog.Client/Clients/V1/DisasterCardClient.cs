using System.Text.Json;
using ThunderbirdsBoardGameEngine.Catalog.Client.Exceptions;
using ThunderbirdsBoardGameEngine.Catalog.Client.Interfaces.V1;
using ThunderbirdsBoardGameEngine.Catalog.Client.Internal.Routing;
using ThunderbirdsBoardGameEngine.Catalog.Client.Internal.Serialization;
using ThunderbirdsBoardGameEngine.Catalog.Contracts.Dtos.V1;

namespace ThunderbirdsBoardGameEngine.Catalog.Client.Clients.V1
{
    public class DisasterCardClient : IDisasterCardClient
    {
        private readonly HttpClient _httpClient;

        public DisasterCardClient(HttpClient httpClient)
        {
            _httpClient = httpClient;

            if (!httpClient.DefaultRequestHeaders.Contains("X-Api-Version"))
            {
                httpClient.DefaultRequestHeaders.Add("X-Api-Version", "1.0");
            }
        }

        public async Task<ApiResult<IReadOnlyList<DisasterCardDto>>> GetAllAsync()
        {
            var response = await _httpClient.GetAsync(ApiRoutes.DisasterCard);
            return await HandleResponse<IReadOnlyList<DisasterCardDto>>(response);
        }

        public async Task<ApiResult<DisasterCardDto>> GetByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"{ApiRoutes.DisasterCard}/{id}");
            return await HandleResponse<DisasterCardDto>(response);
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
