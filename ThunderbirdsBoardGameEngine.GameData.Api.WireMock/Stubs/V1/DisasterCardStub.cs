using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Net;
using ThunderbirdsBoardGameEngine.GameData.Api.Contracts.Dtos.V1;
using WireMock.Matchers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace ThunderbirdsBoardGameEngine.GameData.Api.WireMock.Stubs.V1
{
    public sealed class DisasterCardStub
    {
        public const string Route = "/api/DisasterCard";
        public const string VersionHeader = "X-Api-Version";
        public const string VersionValue = "1.0";
        public const string Json = "application/json; charset=utf-8";
        public const string Text = "text/plain; charset=utf-8";

        private static readonly JsonSerializerSettings JsonSettings = new()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore
        };

        private readonly WireMockServer _server;

        public DisasterCardStub(WireMockServer server) =>
            _server = server ?? throw new ArgumentNullException(nameof(server));

        private static string Serialize(object o) => JsonConvert.SerializeObject(o, JsonSettings);

        private static IRequestBuilder BaseGet(string path) => Request.Create()
            .WithPath(path)
            .WithHeader(VersionHeader, new ExactMatcher(true, VersionValue)) // ignoreCase = true
            .UsingGet();

        private static IResponseBuilder JsonResponse(HttpStatusCode code, object body) => Response.Create()
            .WithStatusCode(code)
            .WithHeader("Content-Type", Json)
            .WithBody(Serialize(body));

        // -------- Success --------

        // GET /api/DisasterCard
        public void RegisterSuccess(IReadOnlyList<DisasterCardDto> dtos) =>
            _server.Given(BaseGet(Route))
                   .RespondWith(JsonResponse(HttpStatusCode.OK, dtos));

        // GET /api/DisasterCard/{id}
        public void RegisterSuccess(int id, DisasterCardDto dto) =>
            _server.Given(BaseGet($"{Route}/{id}"))
                   .RespondWith(JsonResponse(HttpStatusCode.OK, dto));

        public void RegisterSuccess(DisasterCardDto dto) => RegisterSuccess(dto.Id, dto);

        // -------- Not Found (matches current API: text/plain) --------

        public void SimulateNotFound(int id) =>
            _server.Given(Request.Create()
                        .WithPath($"{Route}/{id}")
                        .WithHeader(VersionHeader, new ExactMatcher(true, VersionValue))
                        .UsingGet())
                   .RespondWith(Response.Create()
                        .WithStatusCode(HttpStatusCode.NotFound)
                        .WithHeader("Content-Type", Text)
                        .WithBody("Disaster card not found."));

        // -------- Other errors (JSON { error }) --------

        public void SimulateOtherErrorForGetAll(HttpStatusCode code, string message) =>
            _server.Given(BaseGet(Route))
                   .RespondWith(JsonResponse(code, new { error = message }));

        public void SimulateOtherErrorForGetById(int id, HttpStatusCode code, string message) =>
            _server.Given(BaseGet($"{Route}/{id}"))
                   .RespondWith(JsonResponse(code, new { error = message }));

        // -------- Guards (work for GetAll + GetById) --------

        // 400 when header is MISSING
        public void RegisterMissingHeaderGuard()
        {
            var path = new RegexMatcher($@"^{Route}(?:/\d+)?$");
            _server.Given(Request.Create()
                        .WithPath(path)
                        .WithHeader(VersionHeader, new AbsentMatcher())
                        .UsingGet())
                   .RespondWith(JsonResponse(HttpStatusCode.BadRequest,
                        new { error = $"Missing header '{VersionHeader}'." }));
        }

        // 400 when header value is WRONG
        public void RegisterWrongHeaderGuard()
        {
            var path = new RegexMatcher($@"^{Route}(?:/\d+)?$");
            _server.Given(Request.Create()
                        .WithPath(path)
                        .WithHeader(VersionHeader, new NotMatcher(new ExactMatcher(true, VersionValue)))
                        .UsingGet())
                   .RespondWith(JsonResponse(HttpStatusCode.BadRequest,
                        new { error = $"Incorrect '{VersionHeader}'. Expected '{VersionValue}'." }));
        }
    }
}
