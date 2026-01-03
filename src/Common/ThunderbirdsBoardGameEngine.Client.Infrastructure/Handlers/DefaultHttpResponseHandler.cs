using System.Text.Json;
using ThunderbirdsBoardGameEngine.Client.Infrastructure.Serialization;

namespace ThunderbirdsBoardGameEngine.Client.Infrastructure.Handlers
{
    public class DefaultHttpResponseHandler : IHttpResponseHandler
    {
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
        public async Task<ApiResult<T>> HandleResponseAsync<T>(HttpResponseMessage response, CancellationToken cancellationToken)
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
