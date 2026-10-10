using System.Net;
using ThunderbirdsBoardGameEngine.GameState.Contracts.Dtos.V1;
using ThunderbirdsBoardGameEngine.WireMock.Hosting;
using WireMock.Server;

namespace ThunderbirdsBoardGameEngine.GameState.WireMock.Stubs.V1
{
    public class GameStub : WireMockStubBase
    {
        public const string CreateGameRoute = "/api/games";

        public const string GetGameStateRoute = "/api/games/*";

        public const string MoveThunderbirdMachineRoute = "/api/games/*/thunderbird-machines/*/movements";

        public const string VersionValue = "1.0";

        public GameStub(WireMockServer wireMockServer)
            : base(wireMockServer)
        {
        }

        public void RegisterCreateGameSuccess(GameStateResponseDto? response = null)
        {
            response ??= CreateRepresentativeGameResponse();

            Server
                .Given(CreatePost(CreateGameRoute, VersionValue))
                .RespondWith(CreateJsonResponse(response, HttpStatusCode.Created));
        }

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

        public void RegisterGetGameStateSuccess(GameStateResponseDto? response = null)
        {
            response ??= CreateRepresentativeGameResponse();

            Server
                .Given(CreateGet(GetGameStateRoute, VersionValue))
                .RespondWith(CreateJsonResponse(response, HttpStatusCode.OK));
        }

        public void RegisterGetGameStateNotFound(string? detail = null)
        {
            Server
                .Given(CreateGet(GetGameStateRoute, VersionValue))
                .RespondWith(ProblemJson(
                    "Resource not found",
                    HttpStatusCode.NotFound,
                    detail ?? "The specified game was not found."));
        }

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

        public void RegisterMoveThunderbirdMachineSuccess(GameStateResponseDto? response = null)
        {
            response ??= CreateRepresentativeGameResponse();

            Server
                .Given(CreatePost(MoveThunderbirdMachineRoute, VersionValue))
                .RespondWith(CreateJsonResponse(response, HttpStatusCode.OK));
        }

        public void RegisterMoveThunderbirdMachineNotFound(string? detail = null)
        {
            Server
                .Given(CreatePost(MoveThunderbirdMachineRoute, VersionValue))
                .RespondWith(ProblemJson(
                    "Resource not found",
                    HttpStatusCode.NotFound,
                    detail ?? "The specified Thunderbird machine was not found."));
        }

        public void RegisterMoveThunderbirdMachineInvalidMovement(string? detail = null)
        {
            Server
                .Given(CreatePost(MoveThunderbirdMachineRoute, VersionValue))
                .RespondWith(ProblemJson(
                    "Invalid movement",
                    HttpStatusCode.UnprocessableEntity,
                    detail ?? "The requested movement is invalid."));
        }

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
