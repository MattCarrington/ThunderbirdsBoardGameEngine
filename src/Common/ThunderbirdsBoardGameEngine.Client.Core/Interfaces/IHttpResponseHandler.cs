namespace ThunderbirdsBoardGameEngine.Client.Core.Interfaces
{
    /// <summary>
    /// Defines a contract for handling HTTP responses and converting them into strongly typed API result objects
    /// asynchronously.
    /// </summary>
    /// <remarks>Implementations of this interface are responsible for interpreting the HTTP response and
    /// mapping it to an appropriate result type. This may include deserializing the response content, handling error
    /// conditions, or applying custom logic based on the response status code.</remarks>
    public interface IHttpResponseHandler
    {
        /// <summary>
        /// Processes an HTTP response and returns an <see cref="ApiResult{T}"/> containing
        /// the deserialized content or an error message.
        /// </summary>
        /// <remarks>
        /// If the response indicates success, the content is deserialized to <typeparamref name="T"/>.
        /// If deserialization fails or the content is null, the result indicates failure.
        /// Non-success responses return an error extracted from the response body.
        /// <para>
        /// Implementations must preserve cancellation by rethrowing
        /// <see cref="OperationCanceledException"/>.
        /// </para>
        /// </remarks>
        /// <typeparam name="T">The expected response payload type.</typeparam>
        /// <param name="response">The HTTP response message to process.</param>
        /// <param name="cancellationToken">A token used to cancel the operation.</param>
        /// <returns>
        /// An <see cref="ApiResult{T}"/> containing the deserialized response on success,
        /// or error information on failure.
        /// </returns>
        Task<ApiResult<T>> HandleResponseAsync<T>(HttpResponseMessage response, CancellationToken cancellationToken);
    }
}