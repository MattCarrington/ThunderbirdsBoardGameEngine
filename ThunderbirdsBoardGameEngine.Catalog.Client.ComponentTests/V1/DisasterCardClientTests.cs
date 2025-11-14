using Microsoft.Extensions.DependencyInjection;
using System.Net;
using ThunderbirdsBoardGameEngine.Catalog.Client.Interfaces.V1;
using ThunderbirdsBoardGameEngine.Catalog.Contracts.Dtos.V1;
using ThunderbirdsBoardGameEngine.Catalog.WireMock;
using ThunderbirdsBoardGameEngine.TestUtils;
using ThunderbirdsBoardGameEngine.TestUtils.Catalog.Factories;
using ThunderbirdsBoardGameEngine.TestUtils.Helpers;
using ThunderbirdsBoardGameEngine.TestUtils.xUnit.Assertions;
using ThunderbirdsBoardGameEngine.TestUtils.xUnit.Fixtures;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Catalog.Client.ComponentTests.V1
{
    [Collection("WireMock")]
    public class DisasterCardClientTests : IAsyncLifetime
    {
        private readonly IReadOnlyList<DisasterCardDto> _cards =
            TestDataLoader.LoadJsonFromFile<IReadOnlyList<DisasterCardDto>>("disaster-card-dto-data.json", TestDataConstants.V1InputFolder);

        private readonly WireMockHost _host;
        private readonly IDisasterCardsClient _client;
        private readonly ServiceProvider _sp;

        public Task InitializeAsync()
        {
            return Task.CompletedTask;
        }

        public async Task DisposeAsync()
        {
            await _sp.DisposeAsync();
        }

        public DisasterCardClientTests(WireMockFixture fixture)
        {
            _host = fixture.Host;
            _host.Reset();
            _host.DisasterCardStub.RegisterMissingHeaderGuard();
            _host.DisasterCardStub.RegisterIncorrectHeaderGuard();

            _sp = CatalogClientProviderFactory.Build(_host.Url!);
            _client = _sp.GetRequiredService<IDisasterCardsClient>();
        }

        [Fact]
        public async Task GetAllAsync_WhenCalled_CallsCorrectRouteOnce()
        {
            // Arrange
            _host.DisasterCardStub.RegisterGetAllSuccess(_cards);

            // Act
            _ = await _client.GetAllAsync();

            // Assert
            var hits = _host.DisasterCardStub.GetAllRequestPaths();

            var route = Assert.Single(hits);
            Assert.Equal("/api/disaster-cards", route);
        }

        [Fact]
        public async Task GetAllAsync_WhenValidJson_ReturnsSuccessApiResult()
        {
            // Arrange
            _host.DisasterCardStub.RegisterGetAllSuccess(_cards);

            // Act
            var response = await _client.GetAllAsync();

            // Assert           
            Assert.True(response.Success);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(response.Data);
            Assert.Equal(_cards.Count, response.Data.Count);
            Assert.Null(response.ErrorMessage);
            DisasterCardDtoAssertions.AssertOrderSensitive(_cards.ToList(), response.Data.ToList());
        }

        [Fact]
        public async Task GetAllAsync_WhenEmptyList_ReturnsSuccessApiResult()
        {
            // Arrange
            _host.DisasterCardStub.RegisterGetAllEmpty();

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
            _host.DisasterCardStub.RegisterGetAllError();

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
            _host.DisasterCardStub.RegisterGetAllMalformedJson();

            // Act
            var response = await _client.GetAllAsync();

            // Assert           
            Assert.False(response.Success);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Null(response.Data);
            Assert.NotNull(response.ErrorMessage);
        }

        [Fact]
        public async Task GetAllAsync_WhenConcurrentRequests_ReturnsSuccessApiResultsAsync()
        {
            // Arrange
            _host.DisasterCardStub.RegisterGetAllSuccess(_cards);

            var tasks = Enumerable.Range(0, 20)
                .Select(_ => _client.GetAllAsync())
                .ToArray();

            // Act
            Task.WaitAll(tasks);

            // Assert
            foreach (var task in tasks)
            {
                var response = await task;
                Assert.True(response.Success);
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.NotNull(response.Data);
                Assert.Equal(_cards.Count, response.Data.Count);
                Assert.Null(response.ErrorMessage);
                DisasterCardDtoAssertions.AssertOrderSensitive(_cards.ToList(), response.Data.ToList());
            }
            var hits = _host.DisasterCardStub.GetAllRequestPaths();
            Assert.Equal(20, hits.Count());
        }
    }
}
