using Microsoft.Extensions.DependencyInjection;
using System.Net;
using ThunderbirdsBoardGameEngine.Catalog.Client.Interfaces.V1;
using ThunderbirdsBoardGameEngine.Catalog.WireMock;
using ThunderbirdsBoardGameEngine.TestUtils.Catalog.Factories;
using ThunderbirdsBoardGameEngine.TestUtils.xUnit.Fixtures;
using ThunderbirdsBoardGameEngine.WireMock.Hosting;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Catalog.Client.ComponentTests.V1
{
    [Collection("WireMock")]
    public class CharactersClientTests : IAsyncLifetime
    {
        private readonly WireMockHost _host;
        private readonly ICharactersClient _client;
        private readonly ServiceProvider _sp;

        public Task InitializeAsync()
        {
            return Task.CompletedTask;
        }

        public async Task DisposeAsync()
        {
            await _sp.DisposeAsync();
        }

        public CharactersClientTests(WireMockFixture fixture)
        {
            _host = fixture.Host;
            _host.Reset();
            _host.CharactersStub().RegisterMissingHeaderGuard();
            _host.CharactersStub().RegisterIncorrectHeaderGuard();

            _sp = CatalogClientProviderFactory.Build(_host.Url!);
            _client = _sp.GetRequiredService<ICharactersClient>();
        }

        [Fact]
        public async Task GetAllAsync_WhenCalled_CallsCorrectRouteOnce()
        {
            // Arrange
            _host.CharactersStub().RegisterGetAllSuccess();

            // Act
            _ = await _client.GetAllAsync();

            // Assert
            var hits = _host.DisasterCardStub().GetAllRequestPaths();

            var route = Assert.Single(hits);
            Assert.Equal("/api/catalog/characters", route);
        }

        [Fact]
        public async Task GetAllAsync_WhenValidJson_ReturnsSuccessApiResult()
        {
            // Arrange
            _host.CharactersStub().RegisterGetAllSuccess();

            // Act
            var response = await _client.GetAllAsync();

            // Assert           
            Assert.True(response.Success);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(response.Data);
            Assert.Equal(6, response.Data.Count);
            Assert.Null(response.ErrorMessage);
        }

        [Fact]
        public async Task GetAllAsync_WhenEmptyList_ReturnsSuccessApiResult()
        {
            // Arrange
            _host.CharactersStub().RegisterGetAllEmpty();

            // Act
            var response = await _client.GetAllAsync();

            // Assert           
            Assert.True(response.Success);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(response.Data);
            Assert.Empty(response.Data);
            Assert.Null(response.ErrorMessage);
        }

        [Fact]
        public async Task GetAllAsync_WhenServerError_ReturnsFailureApiResult()
        {
            // Arrange
            _host.CharactersStub().RegisterGetAllError();

            // Act
            var response = await _client.GetAllAsync();

            // Assert           
            Assert.False(response.Success);
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
            Assert.Null(response.Data);
            Assert.NotNull(response.ErrorMessage);
        }

        [Fact]
        public async Task GetAllAsync_WhenInvalidJson_ReturnsFailureApiResult()
        {
            // Arrange
            _host.CharactersStub().RegisterGetAllMalformedJson();

            // Act
            var response = await _client.GetAllAsync();

            // Assert           
            Assert.False(response.Success);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Null(response.Data);
            Assert.NotNull(response.ErrorMessage);
        }
    }
}
