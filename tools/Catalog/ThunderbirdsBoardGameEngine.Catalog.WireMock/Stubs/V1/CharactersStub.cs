using System.Net;
using ThunderbirdsBoardGameEngine.Catalog.Contracts.Dtos.V1;
using ThunderbirdsBoardGameEngine.WireMock.Hosting;
using WireMock.Server;

namespace ThunderbirdsBoardGameEngine.Catalog.WireMock.Stubs.V1
{
    /// <summary>
    /// Provides stub registrations for the characters API endpoint, enabling simulation of various response scenarios
    /// for testing purposes.
    /// </summary>
    /// <remarks>Use this class to configure the WireMock server with predefined responses for the characters
    /// endpoint, including successful, empty, malformed, and error cases. This is intended to facilitate client
    /// integration and error handling tests against the /api/catalog/characters route.</remarks>
    [Obsolete("Catalog API is deprecated. Use Reference Data instead")]
    public class CharactersStub : WireMockStubBase
    {
        /// <summary>
        /// Route for the characters endpoint.
        /// </summary>
        public const string Route = "/api/catalog/characters";

        /// <summary>
        /// Supported API version value.
        /// </summary>
        public const string VersionValue = "1.0";

        /// <summary>
        /// Initializes a new instance of the <see cref="CharactersStub"/> class.
        /// </summary>
        /// <param name="server">
        /// The <see cref="WireMockServer"/> instance to register stubs against.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="server"/> is <c>null</c>.
        /// </exception>
        public CharactersStub(WireMockServer server)
            : base(server)
        {
        }

        /// <summary>
        /// Registers a successful response returning all characters.
        /// </summary>
        public void RegisterGetAllSuccess()
        {
            var characters = new List<CharacterDto>
            {
                new() {
                    Key = "scott",
                    DisplayName = "Scott"
                },
                new() {
                    Key = "virgil",
                    DisplayName = "Virgil"
                },
                new() {
                    Key = "alan",
                    DisplayName = "Alan"
                },
                new() {
                    Key = "gordon",
                    DisplayName = "Gordon"
                },
                new() {
                    Key = "john",
                    DisplayName = "John"
                },
                new() {
                    Key = "lady-penelope",
                    DisplayName = "Lady Penelope"
                }
            };

            Server
                .Given(CreateGet(Route, VersionValue))
                .RespondWith(CreateJsonResponse(characters, HttpStatusCode.OK));
        }

        /// <summary>
        /// Registers a successful response returning an empty result set.
        /// </summary>
        public void RegisterGetAllEmpty()
        {
            Server
                .Given(CreateGet(Route, VersionValue))
                .RespondWith(CreateJsonResponse(Array.Empty<CharacterDto>(), HttpStatusCode.OK));
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
        /// Registers an error response for the characters endpoint.
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
        /// Returns the request paths for all recorded calls.
        /// </summary>
        public IReadOnlyList<string> GetAllRequestPaths()
        {
            return Server.LogEntries.Select(le => le.RequestMessage.Path).ToList();
        }
    }
}
