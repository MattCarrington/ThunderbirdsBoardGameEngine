using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using ThunderbirdsBoardGameEngine.Catalog.Contracts.Dtos.V1;
using ThunderbirdsBoardGameEngine.Catalog.WireMock.Stubs.V1;
using ThunderbirdsBoardGameEngine.TestUtils;
using ThunderbirdsBoardGameEngine.TestUtils.Helpers;
using ThunderbirdsBoardGameEngine.TestUtils.xUnit.Assertions;
using ThunderbirdsBoardGameEngine.TestUtils.xUnit.Fixtures;
using WireMock.Server;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Catalog.WireMock.ComponentTests
{
    [Collection("WireMock")]
    public class DisasterCardStubTests : IAsyncLifetime
    {
        private readonly WireMockServer _server;
        private readonly DisasterCardStub _stub;
        private readonly HttpClient _client;

        private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

        private readonly List<DisasterCardDto> _cards = TestDataLoader.LoadJsonFromFile<List<DisasterCardDto>>("disaster-card-dto-data.json", TestDataConstants.V1InputFolder);

        private const string _json = "application/json";

        public DisasterCardStubTests(WireMockFixture fixture)
        {
            _server = fixture.Host.WireMockServer;
            _server.Reset();

            _stub = fixture.Host.DisasterCardStub;
            _stub.RegisterMissingHeaderGuard();
            _stub.RegisterIncorrectHeaderGuard();

            _client = CreateClient(withVersionHeader: true);
        }

        public Task InitializeAsync()
        {
            return Task.CompletedTask;
        }

        public async Task DisposeAsync()
        {
            _client.Dispose();
            await Task.CompletedTask;
        }

        [Fact]
        public async Task GetAllAsync_WhenRegistered_ReturnsSuccessAsync()
        {
            // Arrange
            _stub.RegisterGetAllSuccess(_cards);

            // Act
            var response = await _client.GetAsync(DisasterCardStub.Route);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(_json, response.Content.Headers.ContentType!.MediaType);

            var result = await response.Content.ReadFromJsonAsync<IReadOnlyList<DisasterCardDto>>(JsonOptions);

            Assert.NotNull(result);
            DisasterCardDtoAssertions.AssertOrderInsensitive(_cards.ToList(), result.ToList());
        }

        [Fact]
        public async Task GetAllAsync_WhenRegisteredEmptyList_ReturnsSuccessAsync()
        {
            // Arrange
            _stub.RegisterGetAllEmpty();

            // Act
            var response = await _client.GetAsync(DisasterCardStub.Route);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(_json, response.Content.Headers.ContentType!.MediaType);

            var result = await response.Content.ReadFromJsonAsync<IReadOnlyList<DisasterCardDto>>(JsonOptions);

            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetAllAsync_WhenRegisteredMalformed_ReturnsSuccessAsync()
        {
            // Arrange
            _stub.RegisterGetAllEmpty();

            // Act
            var response = await _client.GetAsync(DisasterCardStub.Route);

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

            // Act
            var response = await _client.GetAsync(DisasterCardStub.Route);

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

            // Act
            var response = await _client.GetAsync(DisasterCardStub.Route);

            // Assert
            Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);

            var result = await response.Content.ReadAsStringAsync();
            Assert.Contains(errorMessage, result);
        }

        [Fact]
        public async Task GetAllAsync_WhenNoVersionHeader_ReturnsBadRequestAsync()
        {
            // Arrange
            var clientWithoutHeader = CreateClient(withVersionHeader: false);

            _stub.RegisterGetAllSuccess(_cards); // should not be called

            // Act
            var response = await clientWithoutHeader.GetAsync(DisasterCardStub.Route);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            
            var result = await response.Content.ReadAsStringAsync();

            Assert.Contains($"Missing header 'X-Api-Version'", result);
            clientWithoutHeader.Dispose();
        }

        [Fact]
        public async Task GetAllAsync_WhenWrongVersionHeader_ReturnsBadRequestAsync()
        {
            // Arrange
            var clientWithoutHeader = CreateClient(versionHeader: "2.0");

            _stub.RegisterGetAllSuccess(_cards); // should not be called

            // Act
            var response = await clientWithoutHeader.GetAsync(DisasterCardStub.Route);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            
            var result = await response.Content.ReadAsStringAsync();

            Assert.Contains("Unsupported version in header 'X-Api-Version'. Expected '1.0'.", result);
            clientWithoutHeader.Dispose();
        }
        private HttpClient CreateClient(bool withVersionHeader = true, string? versionHeader = DisasterCardStub.VersionValue)
        {
            var client = new HttpClient { BaseAddress = new Uri(_server.Urls[0]) };

            if (withVersionHeader)
            {
                client.DefaultRequestHeaders.Add(DisasterCardStub.VersionHeader, versionHeader);
            }

            return client;
        }
    }
}
