using System.Net;
using ThunderbirdsBoardGameEngine.Catalog.Contracts.Dtos.V1;
using WireMock.Logging;
using WireMock.Matchers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace ThunderbirdsBoardGameEngine.Catalog.WireMock.Stubs.V1
{
    /// <summary>
    /// WireMock stub helper for the Catalog V1 disaster cards endpoint.
    /// </summary>
    /// <remarks>
    /// This stub registers common success and error scenarios for
    /// <c>GET /api/catalog/disaster-cards</c>, including version header
    /// validation and malformed responses.
    ///
    /// Intended for use in integration and consumer tests.
    /// </remarks>
    public sealed class DisasterCardStub
    {
        /// <summary>
        /// Route for the disaster cards endpoint.
        /// </summary>
        public const string Route = "/api/catalog/disaster-cards";

        /// <summary>
        /// API version header name.
        /// </summary>
        public const string VersionHeader = "X-Api-Version";

        /// <summary>
        /// Supported API version value.
        /// </summary>
        public const string VersionValue = "1.0";

        /// <summary>
        /// JSON content type used by the endpoint.
        /// </summary>
        public const string Json = "application/json; charset=utf-8";

        /// <summary>
        /// Plain text content type used for malformed responses.
        /// </summary>
        public const string Text = "text/plain; charset=utf-8";

        private readonly WireMockServer _server;

        /// <summary>
        /// Initializes a new instance of the <see cref="DisasterCardStub"/> class.
        /// </summary>
        /// <param name="server">
        /// The <see cref="WireMockServer"/> instance to register stubs against.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="server"/> is <c>null</c>.
        /// </exception>
        public DisasterCardStub(WireMockServer server)
        {
            _server = server ?? throw new ArgumentNullException(nameof(server));
        }

        /// <summary>
        /// Registers a successful response returning all disaster cards.
        /// </summary>
        /// <param name="dtos">The disaster card DTOs to return.</param>
        public void RegisterGetAllSuccess(IReadOnlyList<DisasterCardDto> dtos)
        {
            _server.Given(CreateRequest()).RespondWith(CreateJsonResponse(dtos, HttpStatusCode.OK));
        }

        /// <summary>
        /// Registers a successful response returning an empty result set.
        /// </summary>
        public void RegisterGetAllEmpty()
        {
            _server.Given(CreateRequest()).RespondWith(CreateJsonResponse(Array.Empty<DisasterCardDto>(), HttpStatusCode.OK));
        }

        /// <summary>
        /// Registers a response that returns malformed JSON.
        /// </summary>
        /// <remarks>
        /// Useful for validating client-side error handling and deserialization failures.
        /// </remarks>
        public void RegisterGetAllMalformedJson()
        {
            _server.Given(CreateRequest()).RespondWith(CreateTextResponse("{ malformed json... ", HttpStatusCode.OK));
        }

        /// <summary>
        /// Registers an error response for the disaster cards endpoint.
        /// </summary>
        /// <param name="status">The HTTP status code to return.</param>
        /// <param name="message">The error message to include in the response body.</param>
        public void RegisterGetAllError(HttpStatusCode status = HttpStatusCode.InternalServerError, string message = "An error occurred")
        {
            var body = new { error = message };

            _server.Given(CreateRequest()).RespondWith(CreateJsonResponse(body, status));
        }

        /// <summary>
        /// Registers a guard that returns a <c>400 Bad Request</c> when the API version header is missing.
        /// </summary>
        public void RegisterMissingHeaderGuard()
        {
            _server
                .Given(Request.Create()
                    .WithPath(new ExactMatcher(true, Route))
                    .UsingGet()
                    .WithHeader(headers =>
                    {
                        if (!headers.TryGetValue(VersionHeader, out var values) || values == null)
                        { 
                            return true; 
                        }

                        return values.Length == 0 || values.All(string.IsNullOrWhiteSpace);
                    }))
                .RespondWith(CreateJsonResponse(new { error = $"Missing header '{VersionHeader}'." }, HttpStatusCode.BadRequest));
        }

        /// <summary>
        /// Registers a guard that returns a <c>400 Bad Request</c> when an unsupported API version is supplied.
        /// </summary>
        public void RegisterIncorrectHeaderGuard()
        {
            _server
                .Given(Request.Create()
                    .WithPath(new ExactMatcher(true, Route))
                    .UsingGet()
                    .WithHeader(dict =>
                    {
                        if (dict == null || !dict.TryGetValue(VersionHeader, out var values) || values == null)
                        { 
                            return false; // let missing-header guard catch it
                        }

                        return !values.Any(v => string.Equals(v, VersionValue, StringComparison.Ordinal));
                    }))
                .RespondWith(CreateJsonResponse(
                    new { error = $"Unsupported version in header '{VersionHeader}'. Expected '{VersionValue}'." },
                    HttpStatusCode.BadRequest));
        }

        /// <summary>
        /// Counts the number of GET requests made to the disaster cards endpoint.
        /// </summary>
        /// <returns>The number of matching requests.</returns>
        public int CountGetAllCalls()
        {
            return _server.LogEntries.Count(le =>
                le.RequestMessage.Method == "GET" &&
                string.Equals(le.RequestMessage.Path, Route, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Returns the request paths for all recorded calls.
        /// </summary>
        public IReadOnlyList<string> GetAllRequestPaths()
        {
            return _server.LogEntries.Select(le => le.RequestMessage.Path).ToList();
        }

        /// <summary>
        /// Gets the most recent request made to the WireMock server.
        /// </summary>
        public ILogEntry? GetLastCall()
        {
            return _server.LogEntries.LastOrDefault();
        }

        private static IRequestBuilder CreateRequest()
        {
            return Request.Create()
                .WithPath(Route)
                .WithHeader(VersionHeader, new ExactMatcher(true, VersionValue))
                .UsingGet();

        }

        private static IResponseBuilder CreateJsonResponse(object body, HttpStatusCode httpStatusCode)
        {
            return Response.Create()
                .WithStatusCode(httpStatusCode)
                .WithHeader("Content-Type", Json)
                .WithBodyAsJson(body);
        }

        private static IResponseBuilder CreateTextResponse(string body, HttpStatusCode httpStatusCode)
        {
            return Response.Create()
                .WithStatusCode(httpStatusCode)
                .WithHeader("Content-Type", Text)
                .WithBody(body);
        }
    }
}
