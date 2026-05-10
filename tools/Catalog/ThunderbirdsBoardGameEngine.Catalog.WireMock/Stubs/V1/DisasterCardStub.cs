using System.Net;
using ThunderbirdsBoardGameEngine.Catalog.Contracts.Dtos.V1;
using ThunderbirdsBoardGameEngine.WireMock.Hosting;
using WireMock.Logging;
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
    [Obsolete("Catalog runtime API is deprecated. Static game data is now provided by ReferenceData snapshots.")]
    public sealed class DisasterCardStub : WireMockStubBase
    {
        /// <summary>
        /// Route for the disaster cards endpoint.
        /// </summary>
        public const string Route = "/api/catalog/disaster-cards";

        /// <summary>
        /// Supported API version value.
        /// </summary>
        public const string VersionValue = "1.0";

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
            : base(server)
        {
        }

        /// <summary>
        /// Registers a successful response returning all disaster cards.
        /// </summary>
        /// <param name="dtos">The disaster card DTOs to return.</param>
        public void RegisterGetAllSuccess(IReadOnlyList<DisasterCardDto> dtos)
        {
            Server
                .Given(CreateGet(Route, VersionValue))
                .RespondWith(CreateJsonResponse(dtos, HttpStatusCode.OK));
        }

        /// <summary>
        /// Registers a successful response returning an empty result set.
        /// </summary>
        public void RegisterGetAllEmpty()
        {
            Server
                .Given(CreateGet(Route, VersionValue))
                .RespondWith(CreateJsonResponse(Array.Empty<DisasterCardDto>(), HttpStatusCode.OK));
        }

        /// <summary>
        /// Registers a response that returns malformed JSON.
        /// </summary>
        /// <remarks>
        /// Useful for validating client-side error handling and deserialization failures.
        /// </remarks>
        public void RegisterGetAllMalformedJson()
        {
            Server
                .Given(CreateGet(Route, VersionValue))
                .RespondWith(ProblemJson("{ malformed json... ", HttpStatusCode.OK));
        }

        /// <summary>
        /// Registers an error response for the disaster cards endpoint.
        /// </summary>
        /// <param name="status">The HTTP status code to return.</param>
        /// <param name="message">The error message to include in the response body.</param>
        /// <param name="detail">Optional detailed error information.</param>
        public void RegisterGetAllError(
            HttpStatusCode status = HttpStatusCode.InternalServerError,
            string message = "An error occurred",
            string detail = "An unknown error occurred on the server")
        {
            Server
                .Given(CreateGet(Route, VersionValue))
                .RespondWith(ProblemJson(message, status, detail));
        }

        /// <summary>
        /// Registers a guard that returns a <c>400 Bad Request</c> when the API version header is missing.
        /// </summary>
        public void RegisterMissingHeaderGuard()
        {
            RegisterMissingVersionHeaderGuard(Route, "GET");
        }

        /// <summary>
        /// Registers a guard that returns a <c>400 Bad Request</c> when an unsupported API version is supplied.
        /// </summary>
        public void RegisterIncorrectHeaderGuard()
        {
            RegisterUnsupportedVersionHeaderGuard(Route, "GET", VersionValue);
        }

        /// <summary>
        /// Counts the number of GET requests made to the disaster cards endpoint.
        /// </summary>
        /// <returns>The number of matching requests.</returns>
        public int CountGetAllCalls()
        {
            return Server.LogEntries.Count(le =>
                le.RequestMessage.Method == "GET" &&
                string.Equals(le.RequestMessage.Path, Route, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Returns the request paths for all recorded calls.
        /// </summary>
        public IReadOnlyList<string> GetAllRequestPaths()
        {
            return Server.LogEntries.Select(le => le.RequestMessage.Path).ToList();
        }

        /// <summary>
        /// Gets the most recent request made to the WireMock server.
        /// </summary>
        public ILogEntry? GetLastCall()
        {
            return Server.LogEntries.LastOrDefault();
        }
    }
}
