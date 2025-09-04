namespace ThunderbirdsBoardGameEngine.Catalog.Client.Handlers
{
    /// <summary>
    /// Adds an <c>X-Api-Version</c> request header for all outgoing requests.
    /// </summary>
    /// <remarks>
    /// This handler enables API versioning to be applied by pipeline registration rather than client code.
    /// </remarks>
    internal sealed class ApiVersionHeaderHandler : DelegatingHandler
    {
        private const string HeaderName = "X-Api-Version";
        private readonly string _value;

        public ApiVersionHeaderHandler(string version)
        {
            _value = string.IsNullOrWhiteSpace(version) ? "1.0" : version;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.Headers.Remove(HeaderName);
            request.Headers.TryAddWithoutValidation(HeaderName, _value);
            return base.SendAsync(request, cancellationToken);
        }
    }
}
