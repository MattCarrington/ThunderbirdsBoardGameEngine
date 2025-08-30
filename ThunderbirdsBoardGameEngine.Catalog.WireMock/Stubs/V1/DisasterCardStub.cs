using System.Net;
using System.Text.RegularExpressions;
using ThunderbirdsBoardGameEngine.Catalog.Contracts.Dtos.V1;
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
            _server.Given(Request.Create()
                        .WithPath(Route)
                        .WithHeader(VersionHeader, new ExactMatcher(true, VersionValue))
                        .UsingGet())
                   .RespondWith(Response.Create()
                        .WithStatusCode(HttpStatusCode.OK)
                        .WithHeader("Content-Type", Json)
                        .WithBodyAsJson(dtos));
        }

        public void RegisterGetByIdSuccess(DisasterCardDto dto)
        {
            _server.Given(Request.Create()
                        .WithPath($"{Route}/{dto.Id}") // exact path for this id
                        .WithHeader(VersionHeader, new ExactMatcher(true, VersionValue))
                        .UsingGet())
                   .RespondWith(Response.Create()
                        .WithStatusCode(HttpStatusCode.OK)
                        .WithHeader("Content-Type", Json)
                        .WithBodyAsJson(dto));
        }

        public void RegisterGetByIdNotFound()
        {
            _server.Given(Request.Create()
                        .WithPath(new RegexMatcher($"^{Regex.Escape(Route)}/\\d+$", ignoreCase: true))
                        .WithHeader(VersionHeader, new ExactMatcher(true, VersionValue))
                        .UsingGet())
                   .RespondWith(Response.Create()
                        .WithStatusCode(HttpStatusCode.NotFound)
                        .WithHeader("Content-Type", Text)
                        .WithBody("Disaster card not found."));
        }

        public void RegisterGetAllError(HttpStatusCode status, string message)
        {
            _server.Given(Request.Create()
                        .WithPath(Route)
                        .WithHeader(VersionHeader, new ExactMatcher(true, VersionValue))
                        .UsingGet())
                   .RespondWith(Response.Create()
                        .WithStatusCode(status)
                        .WithHeader("Content-Type", Json)
                        .WithBodyAsJson(new { error = message }));
        }

        public void RegisterGetByIdError(int id, HttpStatusCode status, string message)
        {
            _server.Given(Request.Create()
                        .WithPath($"{Route}/{id}")
                        .WithHeader(VersionHeader, new ExactMatcher(true, VersionValue))
                        .UsingGet())
                   .RespondWith(Response.Create()
                        .WithStatusCode(status)
                        .WithHeader("Content-Type", Json)
                        .WithBodyAsJson(new { error = message }));
        }

        public void RegisterMissingHeaderGuard()
        {
            var bothRoutes = new RegexMatcher($"^{Regex.Escape(Route)}(?:/\\d+)?$", ignoreCase: true);

            _server.Given(Request.Create()
                .WithPath(bothRoutes)
                .UsingGet()
                .WithHeader(headers =>
                {
                    // Headers type: IDictionary<string, string[]>
                    if (!headers.TryGetValue(VersionHeader, out var values) || values == null)
                        return true; // header entirely missing

                    // present but empty/whitespace only -> treat as missing
                    return values.Length == 0 || values.All(string.IsNullOrWhiteSpace);
                }))
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.BadRequest)
                .WithHeader("Content-Type", Json)
                .WithBodyAsJson(new { error = $"Missing header '{VersionHeader}'." }));
        }

        public void RegisterIncorrectHeaderGuard()
        {
            var bothRoutes = new RegexMatcher($"^{Regex.Escape(Route)}(?:/\\d+)?$", ignoreCase: true);

            _server.Given(Request.Create()
                .WithPath(bothRoutes)
                .UsingGet()
                .WithHeader(headerDict =>
                {
                    // headerDict is IDictionary<string, string[]>
                    if (headerDict == null || !headerDict.TryGetValue(VersionHeader, out var values) || values == null)
                        return false; // let missing header guard handle this

                    // Check if any value matches the expected version
                    return !values.Any(v => string.Equals(v, VersionValue, StringComparison.Ordinal));
                }))
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.BadRequest)
                .WithHeader("Content-Type", Json)
                .WithBodyAsJson(new { error = $"Unsupported version in header '{VersionHeader}'. Expected '{VersionValue}'." }));
        }
    }
}
