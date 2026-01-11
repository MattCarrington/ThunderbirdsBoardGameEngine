using System.Net;
using ThunderbirdsBoardGameEngine.Rules.Contracts.Dtos.Rescue.CalculateRescueTarget.V1;
using ThunderbirdsBoardGameEngine.WireMock.Hosting;
using WireMock.Server;

namespace ThunderbirdsBoardGameEngine.Rules.WireMock.Stubs.V1
{
    /// <summary>
    /// Provides WireMock stub registrations for the rescue target calculation API endpoint, enabling simulation of
    /// various response scenarios for testing purposes.
    /// </summary>
    /// <remarks>Use this class to configure the WireMock server with predefined responses for the
    /// "/api/rules/rescue/*/target" endpoint. It supports simulating successful responses, not found errors, invalid
    /// requests, and server errors, as well as header validation scenarios. This is intended to facilitate integration
    /// and contract testing of clients interacting with the rescue target calculation API.</remarks>
    public class RescueStub : WireMockStubBase
    {
        /// <summary>
        /// Represents the route template for the rescue target API endpoint.
        /// </summary>
        /// <remarks>This constant can be used to configure routing or to reference the endpoint path for
        /// operations related to rescue targets. The asterisk ('*') in the route indicates a placeholder for a variable
        /// segment, such as an identifier.</remarks>
        public const string Route = "/api/rules/rescue/*/target";

        /// <summary>
        /// Represents the current version of the API as a string constant.
        /// </summary>
        public const string VersionValue = "1.0";

        /// <summary>
        /// Initializes a new instance of the RescueStub class using the specified WireMockServer.
        /// </summary>
        /// <param name="wireMockServer">The WireMockServer instance to be used by the stub. Cannot be null.</param>
        public RescueStub(WireMockServer wireMockServer)
            : base(wireMockServer)
        {
        }

        /// <summary>
        /// Registers a successful response for a calculate rescue target request in the test server.
        /// </summary>
        /// <remarks>Use this method to configure the test server to return a specific successful response
        /// when the calculate rescue target API is invoked. This is typically used in integration tests to
        /// simulate server behavior.</remarks>
        /// <param name="response">The response object to be returned when the calculate rescue target endpoint is called. Cannot be null.</param>
        public void RegisterCalculateRescueTargetSuccess(CalculateRescueTargetResponseDto response)
        {
            Server
                .Given(CreatePost(Route, VersionValue))
                .RespondWith(CreateJsonResponse(response, HttpStatusCode.OK));
        }

        /// <summary>
        /// Registers a mock HTTP response for a POST request when a rescue target is not found.
        /// </summary>
        /// <remarks>Use this method in test scenarios to simulate a 404 Not Found response when a
        /// disaster card resource cannot be located. The response includes a problem JSON payload with the specified or
        /// default detail message.</remarks>
        /// <param name="detail">An optional string providing additional details about the not found error. If null, a default message is
        /// used.</param>
        public void RegisterCalculateRescueTargetNotFound(string? detail = null)
        {
            Server
                .Given(CreatePost(Route, VersionValue))
                .RespondWith(ProblemJson(
                    "Resource not found",
                    HttpStatusCode.NotFound,
                    detail ?? "The specified disaster card was not found."));
        }

        /// <summary>
        /// Registers a mock response for an invalid CalculateRescueTarget request, returning a 400 Bad Request error.
        /// </summary>
        /// <param name="detail">An optional string providing additional details about the invalid request. If null, no additional detail is
        /// included in the response.</param>
        public void RegisterInvalidCalculateRescueTargetRequest(string? detail = null)
        {
            Server
                .Given(CreatePost(Route, VersionValue))
                .RespondWith(ProblemJson(
                    "Invalid request",
                    HttpStatusCode.BadRequest,
                    detail));
        }

        /// <summary>
        /// Registers a simulated error response for the CalculateRescueTarget endpoint with the specified HTTP status
        /// code, message, and optional detail.
        /// </summary>
        /// <remarks>Use this method to configure the test server to return a specific error response when
        /// the CalculateRescueTarget endpoint is called. This is useful for testing error handling scenarios in client
        /// applications.</remarks>
        /// <param name="statusCode">The HTTP status code to return in the simulated error response. The default is <see
        /// cref="System.Net.HttpStatusCode.InternalServerError"/>.</param>
        /// <param name="message">The error message to include in the response body. The default is "An unexpected error occurred.".</param>
        /// <param name="detail">Optional additional details to include in the error response. Can be <see langword="null"/>.</param>
        public void RegisterCalculateRescueTargetError(
            HttpStatusCode statusCode = HttpStatusCode.InternalServerError,
            string message = "An unexpected error occurred.",
            string? detail = null)
        {
            Server.
                Given(CreatePost(Route, VersionValue))
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
            RegisterMissingVersionHeaderGuard(Route, "POST");
        }

        /// <summary>
        /// Registers a guard that detects and handles requests with an incorrect or unsupported version header for the
        /// specified route.
        /// </summary>
        public void RegisterIncorrectHeaderGuard()
        {
            RegisterUnsupportedVersionHeaderGuard(Route, "POST", VersionValue);
        }
    }
}
