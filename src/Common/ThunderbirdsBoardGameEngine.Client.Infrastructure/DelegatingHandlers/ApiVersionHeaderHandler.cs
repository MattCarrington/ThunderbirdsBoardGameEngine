namespace ThunderbirdsBoardGameEngine.Client.Infrastructure.DelegatingHandlers
{
    /// <summary>
    /// Adds an <c>X-Api-Version</c> request header for all outgoing requests.
    /// </summary>
    /// <remarks>
    /// This handler enables API versioning to be applied by pipeline registration rather than client code.
    /// </remarks>
    public sealed class ApiVersionHeaderHandler : DelegatingHandler
    {
        private const string HeaderName = "X-Api-Version";
        private readonly string _value;

        /// <summary>
        /// Initializes a new instance of the ApiVersionHeaderHandler class with the specified API version.
        /// </summary>
        /// <param name="version">The API version string to include in the request header. If null, empty, or consists only of white-space
        /// characters, the default version "1.0" is used.</param>
        public ApiVersionHeaderHandler(string version)
        {
            _value = string.IsNullOrWhiteSpace(version) ? "1.0" : version;
        }

        /// <summary>
        /// Sends an HTTP request with a custom header asynchronously as an operation in the HTTP message handler
        /// pipeline.
        /// </summary>
        /// <remarks>This method removes any existing header with the specified name and adds a custom
        /// header to the outgoing request before forwarding it to the next handler in the pipeline.</remarks>
        /// <param name="request">The HTTP request message to send. Cannot be null.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous send operation. The task result contains the HTTP response message.</returns>
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.Headers.Remove(HeaderName);
            request.Headers.TryAddWithoutValidation(HeaderName, _value);
            return base.SendAsync(request, cancellationToken);
        }
    }
}
