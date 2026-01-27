using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using ThunderbirdsBoardGameEngine.Catalog.Contracts.Dtos.V1;
using ThunderbirdsBoardGameEngine.Catalog.WireMock.Stubs.V1;
using ThunderbirdsBoardGameEngine.TestUtils.Catalog.TestFileCatalogs;
using ThunderbirdsBoardGameEngine.TestUtils.Helpers;
using ThunderbirdsBoardGameEngine.TestUtils.xUnit.Assertions;
using ThunderbirdsBoardGameEngine.TestUtils.xUnit.Fixtures;
using WireMock.Server;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Catalog.WireMock.ComponentTests.V1
{
    [Collection("WireMock")]
    public class DisasterCardStubTests
    {
        private readonly WireMockServer _server;
        private readonly DisasterCardStub _stub;

        private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

        private const string _json = "application/json";

        public DisasterCardStubTests(WireMockFixture fixture)
        {
            _server = fixture.Host.WireMockServer;
            _server.Reset();

            _stub = fixture.Host.DisasterCardStub();
            _stub.RegisterMissingHeaderGuard();
            _stub.RegisterIncorrectHeaderGuard();
        }

        [Fact]
        public async Task GetAllAsync_WhenRegistered_ReturnsSuccessAsync()
        {
            // Arrange
            var cards = await GetCardDtosAsync();

            _stub.RegisterGetAllSuccess(cards);

            using var client = CreateClient();

            // Act
            using var response = await client.GetAsync(DisasterCardStub.Route);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(_json, response.Content.Headers.ContentType!.MediaType);

            var result = await response.Content.ReadFromJsonAsync<IReadOnlyList<DisasterCardDto>>(JsonOptions);

            Assert.NotNull(result);
            DisasterCardDtoAssertions.AssertOrderInsensitive(cards.ToList(), result.ToList());
        }

        [Fact]
        public async Task GetAllAsync_WhenRegisteredEmptyList_ReturnsSuccessAsync()
        {
            // Arrange
            _stub.RegisterGetAllEmpty();

            using var client = CreateClient();

            // Act
            using var response = await client.GetAsync(DisasterCardStub.Route);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(_json, response.Content.Headers.ContentType!.MediaType);

            var result = await response.Content.ReadFromJsonAsync<IReadOnlyList<DisasterCardDto>>(JsonOptions);

            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetAllAsync_WhenErrorUnspecified_ReturnsError()
        {
            // Arrange
            _stub.RegisterGetAllError();

            using var client = CreateClient();

            // Act
            using var response = await client.GetAsync(DisasterCardStub.Route);

            // Assert
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);

            var result = await response.Content.ReadAsStringAsync();
            Assert.Contains("An error occurred", result);
        }

        [Fact]
        public async Task GetAllAsync_WhenErrorSpecified_ReturnsError()
        {
            // Arrange
            var errorMessage = "The service is unavailable right now";

            _stub.RegisterGetAllError(HttpStatusCode.ServiceUnavailable, errorMessage);

            using var client = CreateClient();

            // Act
            using var response = await client.GetAsync(DisasterCardStub.Route);

            // Assert
            Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);

            var result = await response.Content.ReadAsStringAsync();
            Assert.Contains(errorMessage, result);
        }

        [Fact]
        public async Task GetAllAsync_WhenNoVersionHeader_ReturnsBadRequestAsync()
        {
            // Arrange
            var cards = await GetCardDtosAsync();

            using var client = new HttpClient { BaseAddress = new Uri(_server.Urls[0]) };

            _stub.RegisterGetAllSuccess(cards); // should not be called

            // Act
            using var response = await client.GetAsync(DisasterCardStub.Route);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            var result = await response.Content.ReadAsStringAsync();

            Assert.Contains($"Missing header 'X-Api-Version'", result);
        }

        [Fact]
        public async Task GetAllAsync_WhenWrongVersionHeader_ReturnsBadRequestAsync()
        {
            // Arrange
            var cards = await GetCardDtosAsync();

            _stub.RegisterGetAllSuccess(cards); // should not be called

            using var client = CreateClient("2.0");

            // Act
            using var response = await client.GetAsync(DisasterCardStub.Route);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            var result = await response.Content.ReadAsStringAsync();

            Assert.Contains("Unsupported version in header 'X-Api-Version'. Expected '1.0'.", result);
        }

        private HttpClient CreateClient(string versionHeader)
        {
            var client = new HttpClient { BaseAddress = new Uri(_server.Urls[0]) };
            client.DefaultRequestHeaders.Add(DisasterCardStub.VersionHeader, versionHeader);

            return client;
        }

        private HttpClient CreateClient()
        {
            return CreateClient(DisasterCardStub.VersionValue);
        }

        private static async Task<IReadOnlyList<DisasterCardDto>> GetCardDtosAsync()
        {
            return await TestDataLoader.LoadJsonFromFileAsync<IReadOnlyList<DisasterCardDto>>(DisasterCardTestFileCatalog.DataOnly("disaster-card-dtos.json"));
        }
    }
}
