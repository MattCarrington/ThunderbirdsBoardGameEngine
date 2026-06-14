using System.Net;
using ThunderbirdsBoardGameEngine.Rules.Contracts.Dtos.Movement.AccessibleLocations.V1;
using ThunderbirdsBoardGameEngine.Rules.Contracts.Dtos.Movement.ValidateMovement.V1;
using ThunderbirdsBoardGameEngine.WireMock.Hosting;
using WireMock.Server;

namespace ThunderbirdsBoardGameEngine.Rules.WireMock.Stubs.V1
{
    /// <summary>
    /// Provides WireMock stub registrations for the movement validation API endpoint, enabling simulation of
    /// various response scenarios for testing purposes.
    /// </summary>
    /// <remarks>Use this class to configure the WireMock server with predefined responses for the
    /// "/api/rules/movement/*/validate" endpoint. It supports simulating successful responses, not found errors, invalid
    /// requests, and server errors, as well as header validation scenarios. This is intended to facilitate integration
    /// and contract testing of clients interacting with the movement validation API.</remarks>

    public class MovementStub : WireMockStubBase
    {
        /// <summary>
        /// Represents the route template for the movement validation API endpoint.
        /// </summary>
        /// <remarks>This constant can be used to configure routing or to reference the endpoint path for
        /// operations related to movement validation. The asterisk ('*') in the route indicates a placeholder for a variable
        /// segment, such as an identifier.</remarks>
        public const string ValidateMovementRoute = "/api/rules/movement/*/validate";

        /// <summary>
        /// Represents the route template for the accessible locations API endpoint.
        /// </summary>
        /// <remarks>This constant can be used to configure routing or to reference the endpoint path for
        /// operations related to accessible locations. The asterisk ('*') in the route indicates a placeholder for a variable
        /// segment, such as an identifier.</remarks>
        public const string GetAccessibleLocationsRoute = "/api/rules/movement/*/accessible-locations";

        /// <summary>
        /// Represents the current version of the API as a string constant.
        /// </summary>
        public const string VersionValue = "1.0";

        /// <summary>
        /// Initializes a new instance of the MovementStub class using the specified WireMockServer.
        /// </summary>
        /// <param name="wireMockServer">The WireMockServer instance to be used for registering stubs.</param>
        public MovementStub(WireMockServer wireMockServer)
            : base(wireMockServer)
        {
        }

        /// <summary>
        /// Registers a stub for a successful movement validation response. When a POST request is made to the configured route
        /// with the correct version header, the stub will respond with the provided <see cref="ValidateMovementResponseDto"/>.
        /// </summary>
        /// <param name="response">The response to be returned for a successful movement validation request.</param>
        public void RegisterValidateMovementSuccess(ValidateMovementResponseDto response)
        {
            Server
                .Given(CreatePost(ValidateMovementRoute, VersionValue))
                .RespondWith(CreateJsonResponse(response, System.Net.HttpStatusCode.OK));
        }

        /// <summary>
        /// Registers a stub for a not found response when validating movement. When a POST request is made to the configured route
        /// with the correct version header, the stub will respond with a not found error.
        /// </summary>
        /// <param name="detail">Optional detail message to include in the response.</param>
        public void RegisterValidateMovementNotFound(string? detail = null)
        {
            Server
                .Given(CreatePost(ValidateMovementRoute, VersionValue))
                .RespondWith(ProblemJson(
                    "Resource not found",
                    HttpStatusCode.NotFound,
                    detail ?? "The specified Thunderbird was not found."));
        }

        /// <summary>
        /// Registers a stub for an invalid movement validation request. When a POST request is made to the configured route
        /// with the correct version header, the stub will respond with a bad request error.
        /// </summary>
        /// <param name="detail">Optional detail message to include in the response.</param>
        public void RegisterInvalidValidateMovementRequest(string? detail = null)
        {
            Server
                .Given(CreatePost(ValidateMovementRoute, VersionValue))
                .RespondWith(ProblemJson(
                    "Invalid request",
                    HttpStatusCode.BadRequest,
                    detail));
        }

        /// <summary>
        /// Registers a stub for an unexpected error during movement validation. When a POST request is made to the configured route
        /// with the correct version header, the stub will respond with the specified error.
        /// </summary>
        /// <param name="statusCode">The HTTP status code to be returned in the response.</param>
        /// <param name="message">The error message to be included in the response.</param>
        /// <param name="detail">Optional detail message to include in the response.</param>
        public void RegisterValidateMovementError(
            HttpStatusCode statusCode = HttpStatusCode.InternalServerError,
            string message = "An unexpected error occurred.",
            string? detail = null)
        {
            Server.
                Given(CreatePost(ValidateMovementRoute, VersionValue))
                .RespondWith(ProblemJson(
                    message,
                    statusCode,
                    detail));
        }

        /// <summary>
        /// Registers a stub for a successful accessible locations response. When a GET request is made to the configured route
        /// with the correct version header, the stub will respond with the provided <see cref="AccessibleLocationsResponseDto"/>.
        /// </summary>
        /// <param name="response">The response to be returned for a successful accessible locations request.</param>
        public void RegisterGetAccessibleLocationsSuccess(AccessibleLocationsResponseDto response)
        {
            Server
                .Given(CreateGet(GetAccessibleLocationsRoute, VersionValue))
                .RespondWith(CreateJsonResponse(response, System.Net.HttpStatusCode.OK));
        }

        /// <summary>
        /// Registers a stub for a not found response when retrieving accessible locations. When a GET request is made to the configured route
        /// with the correct version header, the stub will respond with a not found error.
        /// </summary>
        /// <param name="detail">Optional detail message to include in the response.</param>
        public void RegisterGetAccessibleLocationsNotFound(string? detail = null)
        {
            Server
                .Given(CreateGet(GetAccessibleLocationsRoute, VersionValue))
                .RespondWith(ProblemJson(
                    "Resource not found",
                    HttpStatusCode.NotFound,
                    detail ?? "The specified Thunderbird was not found."));
        }

        /// <summary>
        /// Registers a stub for an unexpected error during accessible locations retrieval. When a GET request is made to the configured route
        /// with the correct version header, the stub will respond with the specified error.
        /// </summary>
        /// <param name="statusCode">The HTTP status code to be returned in the response.</param>
        /// <param name="message">The error message to be included in the response.</param>
        /// <param name="detail">Optional detail message to include in the response.</param>
        public void RegisterGetAccessibleLocationsError(
            HttpStatusCode statusCode = HttpStatusCode.InternalServerError,
            string message = "An unexpected error occurred.",
            string? detail = null)
        {
            Server
                .Given(CreateGet(GetAccessibleLocationsRoute, VersionValue))
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
            RegisterMissingVersionHeaderGuard(ValidateMovementRoute, "POST");
            RegisterMissingVersionHeaderGuard(GetAccessibleLocationsRoute, "GET");
        }

        /// <summary>
        /// Registers a guard that detects and handles requests with an incorrect or unsupported version header for the
        /// specified route.
        /// </summary>
        public void RegisterIncorrectHeaderGuard()
        {
            RegisterUnsupportedVersionHeaderGuard(ValidateMovementRoute, "POST", VersionValue);
            RegisterUnsupportedVersionHeaderGuard(GetAccessibleLocationsRoute, "GET", VersionValue);
        }
    }
}
