using System.Net;
using ThunderbirdsBoardGameEngine.Rules.Contracts.Dtos.Rescue.CalculateRescueTarget.V1;
using ThunderbirdsBoardGameEngine.WireMock.Hosting;
using WireMock.Server;

namespace ThunderbirdsBoardGameEngine.Rules.WireMock.Stubs.V1
{
    public class RescueStub : WireMockStubBase
    {
        public const string Route = "/api/rules/rescue/*/target";

        public const string VersionValue = "1.0";

        public RescueStub(WireMockServer wireMockServer)
            : base(wireMockServer)
        {
        }

        public void RegisterCalculateRescueTargetSuccess(CalculateRescueTargetResponseDto response)
        {
            Server
                .Given(CreatePost(Route, VersionValue))
                .RespondWith(CreateJsonResponse(response, HttpStatusCode.OK));
        }

        public void RegisterCalculateRescueTargetNotFound(string? detail = null)
        {
            Server
                .Given(CreatePost(Route, VersionValue))
                .RespondWith(ProblemJson(
                    "Resource not found",
                    HttpStatusCode.NotFound,
                    detail ?? "The specified disaster card was not found."));
        }

        public void RegisterInvalidCalculateRescueTargetRequest(string? detail = null)
        {
            Server
                .Given(CreatePost(Route, VersionValue))
                .RespondWith(ProblemJson(
                    "Invalid request",
                    HttpStatusCode.BadRequest,
                    detail));
        }

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

        public void RegisterMissingHeaderGuard()
        {
            RegisterMissingVersionHeaderGuard(Route, "POST");
        }

        public void RegisterIncorrectHeaderGuard()
        {
            RegisterUnsupportedVersionHeaderGuard(Route, "POST", VersionValue);
        }
    }
}
