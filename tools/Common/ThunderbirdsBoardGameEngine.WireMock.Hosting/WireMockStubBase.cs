using System.Net;
using WireMock.Matchers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace ThunderbirdsBoardGameEngine.WireMock.Hosting
{
    /// <summary>
    /// Provides a base class for defining WireMock stubs with common configuration and helper methods for request and
    /// response setup.
    /// </summary>
    /// <remarks>This class is intended to be inherited by specific stub implementations that interact with a
    /// WireMockServer instance. It supplies constants for standard headers and content types, as well as protected
    /// helper methods to streamline the creation of JSON and plain text responses. The class enforces the presence of a
    /// valid WireMockServer through its constructor.</remarks>
    public abstract class WireMockStubBase
    {

        /// <summary>
        /// API version header name.
        /// </summary>
        public const string VersionHeader = "X-Api-Version";

        /// <summary>
        /// JSON content type used by the endpoint.
        /// </summary>
        public const string Json = "application/json; charset=utf-8";

        /// <summary>
        /// Provides access to the underlying WireMock server instance used for HTTP request stubbing and verification
        /// in tests.
        /// </summary>
        /// <remarks>This field is intended for use by derived classes to configure, control, or inspect
        /// the WireMock server during test execution. It should not be exposed to external consumers.</remarks>
        protected readonly WireMockServer Server;

        /// <summary>
        /// Initializes a new instance of the WireMockStubBase class with the specified WireMockServer.
        /// </summary>
        /// <param name="server">The WireMockServer instance to associate with this stub. Cannot be null.</param>
        /// <exception cref="ArgumentNullException">Thrown if the server parameter is null.</exception>
        protected WireMockStubBase(WireMockServer server)
        {
            Server = server ?? throw new ArgumentNullException(nameof(server));
        }

        /// <summary>
        /// Creates a GET request builder with the specified route and version header.
        /// </summary>
        /// <param name="route">The relative path for the request. Cannot be null or empty.</param>
        /// <param name="versionValue">The value to set for the version header. Cannot be null.</param>
        /// <returns>An <see cref="IRequestBuilder"/> configured with the specified route and version header, using the GET
        /// method.</returns>
        protected static IRequestBuilder CreateGet(string route, string versionValue)
        {
            return Request.Create()
                .WithPath(route)
                .WithHeader(VersionHeader, new ExactMatcher(true, versionValue))
                .UsingGet();
        }

        protected static IRequestBuilder CreatePost(string route, string versionValue)
        {
            return Request.Create()
                .WithPath(route)
                .WithHeader(VersionHeader, new ExactMatcher(true, versionValue))
                .UsingPost();
        }

        /// <summary>
        /// Creates a response builder that returns the specified object serialized as JSON with the given HTTP status
        /// code.
        /// </summary>
        /// <remarks>The response will include a "Content-Type" header set to "application/json". The
        /// object is serialized using the default JSON serializer settings.</remarks>
        /// <param name="body">The object to serialize as the JSON response body. If null, the response body will be empty.</param>
        /// <param name="httpStatusCode">The HTTP status code to set for the response.</param>
        /// <returns>An <see cref="IResponseBuilder"/> configured to return the specified object as a JSON response with the
        /// provided status code.</returns>
        protected static IResponseBuilder CreateJsonResponse(object body, HttpStatusCode httpStatusCode)
        {
            return Response.Create()
                .WithStatusCode(httpStatusCode)
                .WithHeader("Content-Type", Json)
                .WithBodyAsJson(body);
        }

        /// <summary>
        /// Creates a response with a JSON body formatted according to the Problem Details for HTTP APIs specification
        /// (RFC 7807).
        /// </summary>
        /// <remarks>The response body conforms to the RFC 7807 standard, which enables clients to receive
        /// machine-readable error details in a consistent format. The 'title', 'status', and 'detail' fields are
        /// included in the JSON object as appropriate.</remarks>
        /// <param name="title">A short, human-readable summary of the problem type. This value is included in the problem details object.</param>
        /// <param name="status">The HTTP status code to set in the response and include in the problem details object.</param>
        /// <param name="detail">An optional, human-readable explanation specific to this occurrence of the problem. This value is included
        /// in the problem details object if provided.</param>
        /// <returns>An <see cref="IResponseBuilder"/> configured with the specified status code, a 'Content-Type' header of
        /// 'application/problem+json', and a JSON body containing the problem details.</returns>
        protected static IResponseBuilder ProblemJson(string title, HttpStatusCode status, string? detail = null)
        {
            var problem = new
            {
                title,
                status = (int)status,
                detail
            };

            return Response.Create()
                .WithStatusCode(status)
                .WithHeader("Content-Type", "application/problem+json")
                .WithBodyAsJson(problem);
        }

        /// <summary>
        /// Registers a guard that returns a Bad Request response when the required version header is missing from
        /// requests matching the specified route and HTTP method.
        /// </summary>
        /// <remarks>Use this method to enforce that clients include the required version header in their
        /// requests. Requests that do not include the header, or include it with only whitespace values, will receive a
        /// 400 Bad Request response.</remarks>
        /// <param name="route">The route template to match incoming requests. Must not be null or empty.</param>
        /// <param name="httpMethod">The HTTP method (such as "GET" or "POST") to match for the guard. Must not be null or empty.</param>
        protected void RegisterMissingVersionHeaderGuard(string route, string httpMethod)
        {
            Server
                .Given(Request.Create()
                    .WithPath(new ExactMatcher(true, route))
                    .UsingMethod(httpMethod)
                    .WithHeader(headers =>
                        !headers.ContainsKey(VersionHeader) ||
                        headers[VersionHeader] == null ||
                        headers[VersionHeader].Length == 0 ||
                        headers[VersionHeader].All(string.IsNullOrWhiteSpace)))
                .RespondWith(ProblemJson(
                    "Missing API version header",
                    HttpStatusCode.BadRequest,
                    $"Missing header '{VersionHeader}'."));
        }

        /// <summary>
        /// Registers a guard that returns a Bad Request response when a request to the specified route and HTTP method
        /// contains an unsupported version value in the version header.
        /// </summary>
        /// <remarks>This method is typically used in test scenarios to ensure that requests with invalid
        /// version headers are rejected with a Bad Request response. The response includes a message indicating the
        /// expected version value.</remarks>
        /// <param name="route">The route path to match for incoming requests. Must not be null or empty.</param>
        /// <param name="httpMethod">The HTTP method (such as "GET" or "POST") to match for the request. Must not be null or empty.</param>
        /// <param name="versionValue">The expected version value for the version header. Requests with a different value will trigger the guard.
        /// Must not be null or empty.</param>
        protected void RegisterUnsupportedVersionHeaderGuard(string route, string httpMethod, string versionValue)
        {
            Server
                .Given(Request.Create()
                    .WithPath(route)
                    .UsingMethod(httpMethod)
                    .WithHeader(h =>
                        h.TryGetValue(VersionHeader, out var values) &&
                        values.All(v => v != versionValue)))
                .RespondWith(ProblemJson(
                    $"Unsupported version in header '{VersionHeader}'. Expected '{versionValue}'.",
                    HttpStatusCode.BadRequest));
        }
    }
}