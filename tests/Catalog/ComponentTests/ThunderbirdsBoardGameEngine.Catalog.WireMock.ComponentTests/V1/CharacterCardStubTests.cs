using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using ThunderbirdsBoardGameEngine.Catalog.Contracts.Dtos.V1;
using ThunderbirdsBoardGameEngine.Catalog.WireMock.Stubs.V1;
using ThunderbirdsBoardGameEngine.TestUtils.xUnit.Fixtures;
using WireMock.Server;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Catalog.WireMock.ComponentTests.V1
{
    [Collection("WireMock")]
    public class CharacterCardStubTests
    {
        private readonly WireMockServer _server;
        private readonly CharactersStub _stub;

        private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

        private const string _json = "application/json";

        public CharacterCardStubTests(WireMockFixture fixture)
        {
            _server = fixture.Host.WireMockServer;
            _server.Reset();

            _stub = fixture.Host.CharactersStub();
            _stub.RegisterMissingHeaderGuard();
            _stub.RegisterIncorrectHeaderGuard();
        }

        [Fact]
        public async Task GetAllAsync_WhenRegistered_ReturnsSuccessAsync()
        {
            // Arrange
            _stub.RegisterGetAllSuccess();

            using var client = CreateClient(CharactersStub.VersionValue);

            // Act
            using var response = await client.GetAsync(CharactersStub.Route);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(_json, response.Content.Headers.ContentType!.MediaType);

            var result = await response.Content.ReadFromJsonAsync<IReadOnlyList<CharacterDto>>(JsonOptions);

            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Equal(6, result.Count);

            Assert.Contains(result, c => c.Key == "scott");
            Assert.Contains(result, c => c.Key == "virgil");
            Assert.Contains(result, c => c.Key == "john");
            Assert.Contains(result, c => c.Key == "gordon");
            Assert.Contains(result, c => c.Key == "alan");
            Assert.Contains(result, c => c.Key == "lady-penelope");
        }

        [Fact]
        public async Task GetAllAsync_WhenRegisteredEmptyList_ReturnsSuccessAsync()
        {
            // Arrange
            _stub.RegisterGetAllEmpty();

            using var client = CreateClient();

            // Act
            using var response = await client.GetAsync(CharactersStub.Route);

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
            using var response = await client.GetAsync(CharactersStub.Route);

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
            using var response = await client.GetAsync(CharactersStub.Route);

            // Assert
            Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);

            var result = await response.Content.ReadAsStringAsync();
            Assert.Contains(errorMessage, result);
        }

        [Fact]
        public async Task GetAllAsync_WhenNoVersionHeader_ReturnsBadRequestAsync()
        {
            // Arrange
            using var client = new HttpClient { BaseAddress = new Uri(_server.Urls[0]) };

            _stub.RegisterGetAllSuccess(); // should not be called

            // Act
            using var response = await client.GetAsync(CharactersStub.Route);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            var result = await response.Content.ReadAsStringAsync();

            Assert.Contains($"Missing header 'X-Api-Version'", result);
        }

        [Fact]
        public async Task GetAllAsync_WhenWrongVersionHeader_ReturnsBadRequestAsync()
        {
            // Arrange
            _stub.RegisterGetAllSuccess(); // should not be called

            using var client = CreateClient("2.0");

            // Act
            using var response = await client.GetAsync(CharactersStub.Route);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            var result = await response.Content.ReadAsStringAsync();

            Assert.Contains("Unsupported version in header 'X-Api-Version'. Expected '1.0'.", result);
        }

        private HttpClient CreateClient(string versionValue)
        {
            var client = new HttpClient { BaseAddress = new Uri(_server.Urls[0]) };
            client.DefaultRequestHeaders.Add(CharactersStub.VersionHeader, versionValue);

            return client;
        }

        private HttpClient CreateClient()
        {
            return CreateClient(CharactersStub.VersionValue);
        }
    }
}
