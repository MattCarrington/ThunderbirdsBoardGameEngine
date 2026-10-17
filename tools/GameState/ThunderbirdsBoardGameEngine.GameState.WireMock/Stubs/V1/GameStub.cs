using System.Net;
using ThunderbirdsBoardGameEngine.GameState.Contracts.Dtos.V1;
using ThunderbirdsBoardGameEngine.WireMock.Hosting;
using WireMock.Server;

namespace ThunderbirdsBoardGameEngine.GameState.WireMock.Stubs.V1
{
    /// <summary>
    /// Provides WireMock stub registrations for game-related API endpoints, including creating games, retrieving game
    /// state, and moving Thunderbird machines.
    /// </summary>
    /// <remarks>Configures success, error, and validation responses and registers guards for missing or
    /// unsupported version headers. Defines route and version constants used when creating stub mappings.</remarks>
    public class GameStub : WireMockStubBase
    {
        /// <summary>
        /// Relative route for the API endpoint that creates a game.
        /// </summary>
        /// <remarks>Used to construct request URIs, typically with HTTP POST when creating a new
        /// game.</remarks>
        public const string CreateGameRoute = "/api/games";

        /// <summary>
        /// Route template for retrieving a game's state.
        /// </summary>
        /// <remarks>The trailing '*' is a wildcard that matches the game identifier segment (for example,
        /// '/api/games/123').</remarks>
        public const string GetGameStateRoute = "/api/games/*";

        /// <summary>
        /// Route template for initiating a movement of a Thunderbird machine within a game.
        /// </summary>
        /// <remarks>Contains two wildcard segments for the game and Thunderbird machine identifiers; used
        /// as an API route path template.</remarks>
        public const string MoveThunderbirdMachineRoute = "/api/games/*/thunderbird-machines/*/movements";

        /// <summary>
        /// Identifies the current version of the component.
        /// </summary>
        /// <remarks>Update when the component version changes.</remarks>
        public const string VersionValue = "1.0";

        /// <summary>
        /// Initializes a new instance of the GameStub class using the specified WireMockServer.
        /// </summary>
        /// <remarks>Passes the server instance to the base class constructor.</remarks>
        /// <param name="wireMockServer">The WireMockServer used to register and manage stubbed HTTP requests and responses.</param>
        public GameStub(WireMockServer wireMockServer)
            : base(wireMockServer)
        {
        }

        /// <summary>
        /// Registers a mock HTTP POST handler for the CreateGame route that returns the provided GameStateResponseDto
        /// (or a representative response) with HTTP 201 Created.
        /// </summary>
        /// <remarks>Configures the test HTTP server to respond to POST requests to CreateGameRoute with
        /// VersionValue using a JSON payload and status code 201 Created.</remarks>
        /// <param name="response">The GameStateResponseDto to return in the response; if null, a representative response is generated.</param>
        public void RegisterCreateGameSuccess(GameStateResponseDto? response = null)
        {
            response ??= CreateRepresentativeGameResponse();

            Server
                .Given(CreatePost(CreateGameRoute, VersionValue))
                .RespondWith(CreateJsonResponse(response, HttpStatusCode.Created));
        }

        /// <summary>
        /// Registers a mocked problem+json error response for the create-game endpoint.
        /// </summary>
        /// <remarks>Configures the test server to respond to POST requests to the create-game route with
        /// an application/problem+json payload.</remarks>
        /// <param name="statusCode">HTTP status code to return in the problem response.</param>
        /// <param name="message">Problem title or message included in the response.</param>
        /// <param name="detail">Optional detailed problem description included in the response.</param>
        public void RegisterCreateGameError(
            HttpStatusCode statusCode = HttpStatusCode.InternalServerError,
            string message = "An unexpected error occurred.",
            string? detail = null)
        {
            Server
                .Given(CreatePost(CreateGameRoute, VersionValue))
                .RespondWith(ProblemJson(
                    message,
                    statusCode,
                    detail));
        }

        /// <summary>
        /// Registers a stubbed HTTP 200 (OK) response for the game state GET endpoint, returning the provided
        /// GameStateResponseDto serialized as JSON.
        /// </summary>
        /// <remarks>Configures the test server to match the GET request produced by
        /// CreateGet(GetGameStateRoute, VersionValue) and respond with CreateJsonResponse(response,
        /// HttpStatusCode.OK).</remarks>
        /// <param name="response">The GameStateResponseDto to return as the response body; if null, a representative game response is created
        /// and returned.</param>
        public void RegisterGetGameStateSuccess(GameStateResponseDto? response = null)
        {
            response ??= CreateRepresentativeGameResponse();

            Server
                .Given(CreateGet(GetGameStateRoute, VersionValue))
                .RespondWith(CreateJsonResponse(response, HttpStatusCode.OK));
        }

        /// <summary>
        /// Registers a response mapping for GET requests to the game state route that returns a 404 Not Found with a
        /// problem/json body.
        /// </summary>
        /// <remarks>Uses the configured Server with GetGameStateRoute and VersionValue to match requests.
        /// The response has title "Resource not found" and HTTP status 404.</remarks>
        /// <param name="detail">Optional detail message used as the problem description. If null, the default message "The specified game
        /// was not found." is used.</param>
        public void RegisterGetGameStateNotFound(string? detail = null)
        {
            Server
                .Given(CreateGet(GetGameStateRoute, VersionValue))
                .RespondWith(ProblemJson(
                    "Resource not found",
                    HttpStatusCode.NotFound,
                    detail ?? "The specified game was not found."));
        }

        /// <summary>
        /// Registers a stubbed GET response for the game state endpoint that returns a Problem+JSON payload.
        /// </summary>
        /// <param name="statusCode">HTTP status code to return in the Problem+JSON response (default: InternalServerError).</param>
        /// <param name="message">Problem response title included in the payload (default: "An unexpected error occurred.").</param>
        /// <param name="detail">Optional problem detail included in the payload.</param>
        public void RegisterGetGameStateError(
            HttpStatusCode statusCode = HttpStatusCode.InternalServerError,
            string message = "An unexpected error occurred.",
            string? detail = null)
        {
            Server
                .Given(CreateGet(GetGameStateRoute, VersionValue))
                .RespondWith(ProblemJson(
                    message,
                    statusCode,
                    detail));
        }

        /// <summary>
        /// Registers a test HTTP stub that responds to POST requests to the MoveThunderbirdMachine route with the
        /// provided game state response serialized as JSON and HTTP 200.
        /// </summary>
        /// <remarks>Configures the test server to match CreatePost(MoveThunderbirdMachineRoute,
        /// VersionValue) and respond with CreateJsonResponse(response, HttpStatusCode.OK).</remarks>
        /// <param name="response">Optional game state response to return in the mocked HTTP response; if null, a representative game response
        /// is created.</param>
        public void RegisterMoveThunderbirdMachineSuccess(GameStateResponseDto? response = null)
        {
            response ??= CreateRepresentativeGameResponse();

            Server
                .Given(CreatePost(MoveThunderbirdMachineRoute, VersionValue))
                .RespondWith(CreateJsonResponse(response, HttpStatusCode.OK));
        }

        /// <summary>
        /// Registers a POST stub that responds with a 404 Not Found Problem JSON for the MoveThunderbirdMachine route.
        /// </summary>
        /// <param name="detail">Optional problem-detail message; defaults to "The specified Thunderbird machine was not found."</param>
        public void RegisterMoveThunderbirdMachineNotFound(string? detail = null)
        {
            Server
                .Given(CreatePost(MoveThunderbirdMachineRoute, VersionValue))
                .RespondWith(ProblemJson(
                    "Resource not found",
                    HttpStatusCode.NotFound,
                    detail ?? "The specified Thunderbird machine was not found."));
        }

        /// <summary>
        /// Registers a POST stub that responds with a 422 Unprocessable Entity Problem JSON for the MoveThunderbirdMachine route.
        /// </summary>
        /// <param name="detail">Optional problem-detail message; defaults to "The requested movement is invalid."</param>
        public void RegisterMoveThunderbirdMachineInvalidMovement(string? detail = null)
        {
            Server
                .Given(CreatePost(MoveThunderbirdMachineRoute, VersionValue))
                .RespondWith(ProblemJson(
                    "Invalid movement",
                    HttpStatusCode.UnprocessableEntity,
                    detail ?? "The requested movement is invalid."));
        }

        /// <summary>
        /// Registers a test HTTP response that returns a Problem JSON payload for POST requests to the
        /// MoveThunderbirdMachine route.
        /// </summary>
        /// <remarks>Configures the test server to respond to POST requests for the MoveThunderbirdMachine
        /// route using the provided status, message, and optional detail.</remarks>
        /// <param name="statusCode">HTTP status code to return in the problem response.</param>
        /// <param name="message">Problem title or message to include in the Problem JSON response.</param>
        /// <param name="detail">Optional problem detail to include in the Problem JSON response.</param>
        public void RegisterMoveThunderbirdMachineError(
            HttpStatusCode statusCode = HttpStatusCode.InternalServerError,
            string message = "An unexpected error occurred.",
            string? detail = null)
        {
            Server
                .Given(CreatePost(MoveThunderbirdMachineRoute, VersionValue))
                .RespondWith(ProblemJson(
                    message,
                    statusCode,
                    detail));
        }

        /// <summary>
        /// Registers a guard that enforces the presence of a required version header for POST requests on the
        /// configured route.
        /// </summary>
        public void RegisterMissingHeaderGuard()
        {
            RegisterMissingVersionHeaderGuard(CreateGameRoute, "POST");
            RegisterMissingVersionHeaderGuard(GetGameStateRoute, "GET");
            RegisterMissingVersionHeaderGuard(MoveThunderbirdMachineRoute, "POST");
        }

        /// <summary>
        /// Registers a guard that detects and handles requests with an incorrect or unsupported version header for the
        /// specified route.
        /// </summary>
        public void RegisterIncorrectHeaderGuard()
        {
            RegisterUnsupportedVersionHeaderGuard(CreateGameRoute, "POST", VersionValue);
            RegisterUnsupportedVersionHeaderGuard(GetGameStateRoute, "GET", VersionValue);
            RegisterUnsupportedVersionHeaderGuard(MoveThunderbirdMachineRoute, "POST", VersionValue);
        }

        private static GameStateResponseDto CreateRepresentativeGameResponse()
        {
            return new()
            {
                GameId = Guid.NewGuid(),

                ThunderbirdMachines =
                [
                    new ThunderbirdMachineStateDto
                    {
                        ThunderbirdCode = "thunderbird-1",
                        LocationCode = "south-america",
                        OccupantCharacterCodes = [ "scott" ]
                    },
                    new ThunderbirdMachineStateDto
                    {
                        ThunderbirdCode = "thunderbird-2",
                        LocationCode = "north-america",
                        OccupantCharacterCodes = [ "virgil", "gordon" ]
                    }
                ]
            };
        }
    }
}
