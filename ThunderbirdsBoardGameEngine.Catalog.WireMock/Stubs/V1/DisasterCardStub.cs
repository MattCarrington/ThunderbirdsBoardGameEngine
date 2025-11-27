using System.Net;
using ThunderbirdsBoardGameEngine.Catalog.Contracts.Dtos.V1;
using WireMock.Logging;
using WireMock.Matchers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace ThunderbirdsBoardGameEngine.Catalog.WireMock.Stubs.V1
{
    public sealed class DisasterCardStub
    {
        public const string Route = "/api/disaster-cards";
        public const string VersionHeader = "X-Api-Version";
        public const string VersionValue = "1.0";
        public const string Json = "application/json; charset=utf-8";
        public const string Text = "text/plain; charset=utf-8";

        private readonly WireMockServer _server;

        public DisasterCardStub(WireMockServer server)
        {
            _server = server ?? throw new ArgumentNullException(nameof(server));
        }

        public void RegisterGetAllSuccess(IReadOnlyList<DisasterCardDto> dtos)
        {
            _server.Given(CreateRequest()).RespondWith(CreateJsonResponse(dtos, HttpStatusCode.OK));
        }

        public void RegisterGetAllEmpty()
        {
            _server.Given(CreateRequest()).RespondWith(CreateJsonResponse(Array.Empty<DisasterCardDto>(), HttpStatusCode.OK));
        }

        public void RegisterGetAllMalformedJson()
        {
            _server.Given(CreateRequest()).RespondWith(CreateTextResponse("{ malformed json... ", HttpStatusCode.OK));
        }

        public void RegisterGetAllError(HttpStatusCode status = HttpStatusCode.InternalServerError, string message = "An error occurred")
        {
            var body = new { error = message };

            _server.Given(CreateRequest()).RespondWith(CreateJsonResponse(body, status));
        }

        public void RegisterMissingHeaderGuard()
        {
            _server
                .Given(Request.Create()
                    .WithPath(new ExactMatcher(true, Route))
                    .UsingGet()
                    .WithHeader(headers =>
                    {
                        if (!headers.TryGetValue(VersionHeader, out var values) || values == null)
                            return true;
                        return values.Length == 0 || values.All(string.IsNullOrWhiteSpace);
                    }))
                .RespondWith(CreateJsonResponse(new { error = $"Missing header '{VersionHeader}'." }, HttpStatusCode.BadRequest));
        }

        public void RegisterIncorrectHeaderGuard()
        {
            _server
                .Given(Request.Create()
                    .WithPath(new ExactMatcher(true, Route))
                    .UsingGet()
                    .WithHeader(dict =>
                    {
                        if (dict == null || !dict.TryGetValue(VersionHeader, out var values) || values == null)
                            return false; // let missing-header guard catch it
                        return !values.Any(v => string.Equals(v, VersionValue, StringComparison.Ordinal));
                    }))
                .RespondWith(CreateJsonResponse(
                    new { error = $"Unsupported version in header '{VersionHeader}'. Expected '{VersionValue}'." },
                    HttpStatusCode.BadRequest));
        }

        public int CountGetAllCalls()
        {
            return _server.LogEntries.Count(le =>
                le.RequestMessage.Method == "GET" &&
                string.Equals(le.RequestMessage.Path, Route, StringComparison.OrdinalIgnoreCase));
        }

        public IReadOnlyList<string> GetAllRequestPaths()
        {
            return _server.LogEntries.Select(le => le.RequestMessage.Path).ToList();
        }

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
