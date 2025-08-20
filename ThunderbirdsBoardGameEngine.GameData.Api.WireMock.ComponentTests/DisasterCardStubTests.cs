using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using ThunderbirdsBoardGameEngine.GameData.Api.Contracts.Dtos.V1;
using ThunderbirdsBoardGameEngine.GameData.Api.WireMock.Stubs.V1;
using ThunderbirdsBoardGameEngine.TestUtils;
using ThunderbirdsBoardGameEngine.TestUtils.Assertions;
using ThunderbirdsBoardGameEngine.TestUtils.Helpers;
using WireMock.Server;
using Xunit;

namespace ThunderbirdsBoardGameEngine.GameData.Api.WireMock.ComponentTests
{
    public class DisasterCardStubTests : IAsyncLifetime
    {
        private WireMockServer _server = default!;
        private DisasterCardStub _stub = default!;
        private HttpClient _client = default!;

        private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        private readonly IReadOnlyList<DisasterCardDto> _cards =
            TestDataLoader.LoadJsonFromFile<IReadOnlyList<DisasterCardDto>>("disaster-card-dto-data.json", TestDataConstants.V1InputFolder)
                ?? throw new NullReferenceException();

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

        private HttpClient CreateClient(bool withVersionHeader)
        {
            var client = new HttpClient { BaseAddress = new Uri(_server.Urls[0]) };

            if (withVersionHeader)
            { 
                client.DefaultRequestHeaders.Add(DisasterCardStub.VersionHeader, DisasterCardStub.VersionValue); 
            }

            return client;
        }

        [Fact]
        public async Task GetAllAsync_WithValidCards_ReturnsSuccessAndCardDtos()
        {
            // Arrange
            _stub.RegisterGetAllSuccess(_cards);

            // Act
            var response = await _client.GetAsync(DisasterCardStub.Route);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("application/json", response.Content.Headers.ContentType!.MediaType);

            var result = await response.Content.ReadFromJsonAsync<IReadOnlyList<DisasterCardDto>>(JsonOptions);
            
            Assert.NotNull(result);
            DisasterCardDtoAssertions.AssertOrderInsensitive(_cards.ToList(), result.ToList());
        }

        [Fact]
        public async Task GetByIdAsync_WithValidCard_ReturnsSuccessAndCardDto()
        {
            // Arrange
            var card = _cards[0];

            _stub.RegisterGetByIdSuccess(card);
            
            // Act
            var response = await _client.GetAsync($"{DisasterCardStub.Route}/1");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var result = await response.Content.ReadFromJsonAsync<DisasterCardDto>(JsonOptions);
            
            Assert.NotNull(result);
            DisasterCardDtoAssertions.AssertEqual(card, result);
        }

        [Fact]
        public async Task GetByIdAsync_WhenCardDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            _stub.RegisterGetByIdNotFound();

            // Act
            var response = await _client.GetAsync($"{DisasterCardStub.Route}/999");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            Assert.Equal("text/plain", response.Content.Headers.ContentType!.MediaType);

            var result = await response.Content.ReadAsStringAsync();
            Assert.Contains("not found", result, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task GetByIdAsync_WhenError_ReturnsInternalServerError()
        {
            // Arrange
            _stub.RegisterGetByIdError(2, HttpStatusCode.InternalServerError, "Database unavailable");

            // Act
            var response = await _client.GetAsync($"{DisasterCardStub.Route}/2");

            // Assert
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
            Assert.Equal("application/json", response.Content.Headers.ContentType!.MediaType);
            
            var result = await response.Content.ReadAsStringAsync();
            
            Assert.Contains("Database unavailable", result);
        }

        [Fact]
        public async Task MissingVersionHeader_Returns400()
        {
            // Arrange
            _stub.RegisterMissingHeaderGuard();

            using var client = CreateClient(withVersionHeader: false);

            // Act
            var resp = await client.GetAsync($"{DisasterCardStub.Route}/1");

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
        }
    }
}
