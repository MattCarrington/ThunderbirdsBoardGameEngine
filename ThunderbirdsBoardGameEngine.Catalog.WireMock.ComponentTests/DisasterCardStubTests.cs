using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using ThunderbirdsBoardGameEngine.Catalog.Contracts.Dtos.V1;
using ThunderbirdsBoardGameEngine.Catalog.WireMock.Stubs.V1;
using ThunderbirdsBoardGameEngine.TestUtils;
using ThunderbirdsBoardGameEngine.TestUtils.Assertions;
using ThunderbirdsBoardGameEngine.TestUtils.Helpers;
using WireMock.Server;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Catalog.WireMock.ComponentTests
{
    public class DisasterCardStubTests : IAsyncLifetime
    {
        private WireMockServer _server = default!;
        private DisasterCardStub _stub = default!;
        private HttpClient _client = default!;

        private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

        private readonly List<DisasterCardDto> _cards = TestDataLoader.LoadJsonFromFile<List<DisasterCardDto>>("disaster-card-dto-data.json", TestDataConstants.V1InputFolder);

        private const string _json = "application/json";

        public Task InitializeAsync()
        {
            _server = WireMockServer.Start();
            _stub = new DisasterCardStub(_server);
            _client = CreateClient(withVersionHeader: true);
            return Task.CompletedTask;
        }

        public Task DisposeAsync()
        {
            _client.Dispose();
            _server.Stop();
            _server.Dispose();
            return Task.CompletedTask;
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
        public async Task GetAllAsync_WhenError_ReturnsError()
        {
            // Arrange
            _stub.RegisterGetAllError(HttpStatusCode.InternalServerError, "An error occured");

            // Act
            var response = await _client.GetAsync(DisasterCardStub.Route);

            // Assert
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);

            var result = await response.Content.ReadAsStringAsync();
            Assert.Contains("An error occured", result);
        }

        [Fact]
        public async Task GetAllAsync_WhenNoVersionHeader_ReturnsBadRequestAsync()
        {
            // Arrange
            var clientWithoutHeader = CreateClient(withVersionHeader: false);

            _stub.RegisterMissingHeaderGuard();
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

            _stub.RegisterIncorrectHeaderGuard();
            _stub.RegisterGetAllSuccess(_cards); // should not be called

            // Act
            var response = await clientWithoutHeader.GetAsync(DisasterCardStub.Route);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            
            var result = await response.Content.ReadAsStringAsync();

            Assert.Contains("Unsupported version in header 'X-Api-Version'. Expected '1.0'.", result);
            clientWithoutHeader.Dispose();
        }

        [Fact]
        public async Task GetByIdAsync_WhenRegistered_ReturnsSingleDto()
        {
            // Arrange
            var expected = _cards[0]; // pick any one

            _stub.RegisterGetByIdSuccess(expected);

            // Act
            var response = await _client.GetAsync($"{DisasterCardStub.Route}/{expected.Id}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(_json, response.Content.Headers.ContentType!.MediaType);

            // Use Web defaults or your fixed JsonOptions that are case-insensitive
            var result = await response.Content.ReadFromJsonAsync<DisasterCardDto>(JsonOptions);

            Assert.NotNull(result);
            DisasterCardDtoAssertions.AssertEqual(expected, result);
        }

        [Fact]
        public async Task GetByIdAsync_WhenNotFound_ReturnsNotFound()
        {
            // Arrange
            _stub.RegisterGetByIdNotFound();

            var nonExistentId = 9999;

            // Act
            var response = await _client.GetAsync($"{DisasterCardStub.Route}/{nonExistentId}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            Assert.Equal("text/plain", response.Content.Headers.ContentType!.MediaType);

            var result = await response.Content.ReadAsStringAsync();
            Assert.Equal("Disaster card not found.", result);
        }

        [Fact]
        public async Task GetByIdAsync_WhenError_ReturnsError()
        {
            // Arrange
            var nonExistentId = 9999;

            _stub.RegisterGetByIdError(nonExistentId, HttpStatusCode.InternalServerError, "An error occured");

            // Act
            var response = await _client.GetAsync($"{DisasterCardStub.Route}/{nonExistentId}");

            // Assert
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
            var result = await response.Content.ReadAsStringAsync();
            Assert.Contains("An error occured", result);
        }

        [Fact]
        public async Task GetByIdAsync_WhenNoVersionHeader_ReturnsBadRequestAsync()
        {
            // Arrange
            var clientWithoutHeader = CreateClient(withVersionHeader: false);

            _stub.RegisterMissingHeaderGuard();
            _stub.RegisterGetByIdSuccess(_cards[0]); // should not be called

            // Act
            var response = await clientWithoutHeader.GetAsync(DisasterCardStub.Route);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            var result = await response.Content.ReadAsStringAsync();

            Assert.Contains($"Missing header 'X-Api-Version'", result);
            clientWithoutHeader.Dispose();
        }

        [Fact]
        public async Task GetByIdAsync_WhenWrongVersionHeader_ReturnsBadRequestAsync()
        {
            // Arrange
            var clientWithoutHeader = CreateClient(versionHeader: "2.0");

            _stub.RegisterIncorrectHeaderGuard();
            _stub.RegisterGetByIdSuccess(_cards[0]); // should not be called

            // Act
            var response = await clientWithoutHeader.GetAsync(DisasterCardStub.Route);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            var result = await response.Content.ReadAsStringAsync();

            Assert.Contains("Unsupported version in header 'X-Api-Version'. Expected '1.0'.", result);
            clientWithoutHeader.Dispose();
        }
    }
}
