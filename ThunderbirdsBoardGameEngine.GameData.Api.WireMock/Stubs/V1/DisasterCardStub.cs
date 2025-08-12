using System.Net;
using ThunderbirdsBoardGameEngine.GameData.Api.Contracts.Dtos.V1;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace ThunderbirdsBoardGameEngine.GameData.Api.WireMock.Stubs.V1
{
    public class DisasterCardStub
    {
        private readonly WireMockServer _wireMockServer;

        public DisasterCardStub(WireMockServer wireMockServer)
        {
            _wireMockServer = wireMockServer;
        }

        public void GetAllAsyncSuccess(IReadOnlyList<DisasterCardDto> disasterCardDtos)
        {
            _wireMockServer.Given(Request.Create().WithPath("/api/DisasterCard").WithHeader("X-Api-Version", "1.0").UsingGet())
                .RespondWith(Response.Create()
                    .WithStatusCode(HttpStatusCode.OK)
                    .WithHeader("Content-Type", "application/json")
                    .WithBodyAsJson(disasterCardDtos));
        }

        public void GetByIdAsyncSuccess(DisasterCardDto disasterCardDto)
        {
            _wireMockServer.Given(Request.Create().WithPath($"/api/DisasterCard/{disasterCardDto.Id}").WithHeader("X-Api-Version", "1.0").UsingGet())
                .RespondWith(Response.Create()
                    .WithStatusCode(HttpStatusCode.OK)
                    .WithHeader("Content-Type", "application/json")
                    .WithBodyAsJson(disasterCardDto));
        }

        public void GetAllAsyncError(HttpStatusCode statusCode, string errorMessage)
        {
            _wireMockServer.Given(Request.Create().WithPath("/api/DisasterCard").WithHeader("X-Api-Version", "1.0").UsingGet())
                .RespondWith(Response.Create()
                    .WithStatusCode(statusCode)
                    .WithHeader("Content-Type", "application/json")
                    .WithBodyAsJson(new { Error = errorMessage }));
        }
        
        public void GetByIdAsyncError(int id, HttpStatusCode statusCode, string errorMessage)
        {
            _wireMockServer.Given(Request.Create().WithPath($"/api/DisasterCard/{id}").WithHeader("X-Api-Version", "1.0").UsingGet())
                .RespondWith(Response.Create()
                    .WithStatusCode(statusCode)
                    .WithHeader("Content-Type", "application/json")
                    .WithBodyAsJson(new { Error = errorMessage }));
        }
    }
}