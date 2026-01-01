using System.Text.Json;
using ThunderbirdsBoardGameEngine.Client.Infrastructure.Serialization;

namespace ThunderbirdsBoardGameEngine.Client.Infrastructure
{
    /// <summary>
    /// Provides a base class for API client implementations that use an underlying HttpClient for making HTTP requests.
    /// </summary>
    /// <remarks>This class is intended to be inherited by concrete API client classes that encapsulate HTTP
    /// communication logic. It manages the lifetime of the provided HttpClient instance and offers common functionality
    /// for handling HTTP responses. Derived classes should use the protected members to implement specific API
    /// operations.</remarks>
    public abstract class ApiClientBase
    {
        /// <summary>
        /// Provides an HTTP client instance for sending HTTP requests and receiving responses from a resource
        /// identified by a URI.
        /// </summary>
        /// <remarks>Intended for use by derived classes to perform HTTP operations. The lifetime of the
        /// client should be managed carefully to avoid socket exhaustion.</remarks>
        protected readonly HttpClient _httpClient;

        /// <summary>
        /// Initializes a new instance of the ApiClientBase class with the specified HTTP client.
        /// </summary>
        /// <param name="httpClient">The HttpClient instance used to send HTTP requests. Cannot be null.</param>
        protected ApiClientBase(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>
        /// Processes an HTTP response and returns an ApiResult containing the deserialized content or an error message.
        /// </summary>
        /// <remarks>If the response indicates success, the method attempts to deserialize the response
        /// content to the specified type. If deserialization fails or the content is null, the result will indicate
        /// failure. If the response is not successful, the error message is extracted from the response content. The
        /// method preserves cancellation requests by rethrowing OperationCanceledException.</remarks>
        /// <typeparam name="T">The type to which the response content is deserialized.</typeparam>
        /// <param name="response">The HTTP response message to process. Must not be null.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an ApiResult with the
        /// deserialized content of type T if the response is successful; otherwise, an ApiResult containing an error
        /// message.</returns>
        protected static async Task<ApiResult<T>> HandleResponse<T>(HttpResponseMessage response, CancellationToken cancellationToken)
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